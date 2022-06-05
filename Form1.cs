using System.Text;
using System.Numerics;
namespace kp
{
    public partial class Form1 : Form
    {                           //алфавит для шифрования
        char[] alphabet = new char[] { '1','2','3','4','5','6','7','8','9','0','-','+','!','@','#','$','%','^','&','*','(',')','_','=','q','w','e','r','t','y','u','i','o','p','[',']'
        ,'a','s','d','f','g','h','j','k','l',';',' ','z','x','c','v','b','n','m',',','.','/','Q','W','E','R','T','Y','U','I','O','P','{','}','A','S','D','F','G','H','J','K','L'
        ,':','"','\\','\'','|','Z','X','C','V','B','N','M','<','>','?','й','ц','у','к','е','н','г','ш','щ','з','х','ъ','ф','ы','в','а','п','р','о','л','д','ж','э','я','ч','с','м',
        'и','т','ь','б','ю','/','Й','Ц','У','К','Е','Н','Г','Ш','Щ','З','Х','Ъ','Ф','Ы','В','А','П','Р','О','Л','Д','Ж','Э','Я','Ч','С','М','И','Т','Ь','Б','Ю','Ё','ё','~','`','№','\n','\r','\u2386'};
        int p, q;               //простые числа
        string filename = "";   //название файла
        public Form1()
        {
            InitializeComponent();
            radioButton1.Checked = true;
            radioButton1.Visible = false;
            radioButton2.Visible = false;
            button2.Visible = false;
            button3.Visible = false;
            openFileDialog1.DefaultExt = "txt";
            saveFileDialog1.DefaultExt = "txt";
            label1.Visible = false;
            label2.Visible = false;
            textBox2.Visible = false;
            textBox3.Visible = false;
        }

        private void button2_Click(object sender, EventArgs e) //кнопка "Обработать данные"
        {
            Random rnd = new Random();
            if (radioButton1.Checked)                //если кодируем текст
            {
                p = rnd.Next(20, 50);                //генерируем случайные
                q = rnd.Next(50, 100);               //числа p и q, достаточные для шифрования
                while (!isPrime(p))                  //и проверяем их на условие простого
                    p++;
                while (!isPrime(q))                  //если они не простые, прибавляем 1 и проверяем снова
                    q++;
                int n = p * q;                       //вычисляем произведение простых чисел
                int eul = (p - 1) * (q - 1);         //вычисляем функцию Эйлера
                int ex = rnd.Next(3, 100);           //находим открытую экспоненту
                while (!isCoprime(ex, eul))          //проверяем её на взаимную простоту
                    ex = rnd.Next(3, 100);           //со значением функции Эйлера
                int secret = modInverse(ex, eul);    //вычисляем закрытую экспоненту
                string m = "";                       //создаём строку, в которую считываем содержимое textBox1
                for (int i = 0; i < textBox1.Lines.Length; i++)
                    m += textBox1.Lines[i].ToString() + " ";
                byte[] data = new byte[m.Length];    //создаём массив 8-битных целочисленных значений, т.к. алфавит
                textBox1.Text = "";                  //не превышает 256 символов, в массив
                for (int i = 0; i < m.Length; i++)   //посимвольно кодируется вся строка с содержимым textBox1
                {
                    data[i] = Convert.ToByte(Array.IndexOf(alphabet, m[i])); //здесь значения массива шифруются
                    BigInteger raw = new BigInteger(data[i]);                //открытым ключом RSA и выводятся в textBox1
                    BigInteger ciphered = (BigInteger.Pow(raw, ex)) % n;     //BigInteger используется, чтобы вместить достаточно
                    textBox1.Text += ciphered.ToString() + " ";              //большие степени, при некоторых P и Q любой стандартный 
                }                                                            //целочисленный тип переполняется
                label1.Visible = true;
                textBox2.Visible = true;
                textBox2.Text = ex.ToString() + " " + n.ToString();          //в textBox2 выводится открытый ключ RSA
                MessageBox.Show("Закрытый ключ для этого сеанса шифрования: " + secret.ToString() + " " + n.ToString() + ", сохраните или запишите его для последующей расшифровки данного файла!", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            if (radioButton2.Checked)           //если декодируем текст
            {
                try
                {
                    string[] SecrAndN = textBox3.Text.Split(' ');           //считывается закрытый ключ, введённый пользователем
                    int encsecret = Convert.ToInt32(SecrAndN[0].Trim());
                    int encn = Convert.ToInt32(SecrAndN[1].Trim());
                    string[] m = textBox1.Text.Split(' ');                  //считывается содержимое textBox1 - зашифрованный текст
                    byte[] data = new byte[m.Length - 1];                   //создаётся массив 8-битных целых чисел для расшифровки
                    for (int i = 0; i < m.Length - 1; i++)                  //и далее текст дешифруется по закрытому ключу RSA
                    {
                        Int16 ciphered = Int16.Parse(m[i]);                 //используется BigInteger по той же причине, что и выше:
                        BigInteger mmmm = (BigInteger.Pow(ciphered, encsecret)) % encn; //ни в один из стандартных целочисленных типов
                        byte raw = (byte)mmmm;                              //не вмещаются большие степени
                        data[i] = Convert.ToByte(raw);
                    }
                    string res = "";                                        //после дешифровки создаётся строка, в которую
                    for (int i = 0; i < data.Length; i++)                   //декодируется по алфавиту содержимое массива
                        res += alphabet[data[i]];                           //8-битных чисел, декодированная строка
                    textBox1.Text = res;                                    //записывается в textBox1
                }
                catch
                {
                    MessageBox.Show("Ошибка декодирования текста","Внимание!",MessageBoxButtons.OK,MessageBoxIcon.Error);
                }
            }
        }
        private void button3_Click(object sender, EventArgs e)          //кнопка "Сохранить как"
        {
            {                                                           //путь для сохранения выбирается пользователем
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)    //через диалоговое окно
                    filename = saveFileDialog1.FileName;
                try
                {
                    using (System.IO.StreamWriter str = new StreamWriter(File.Open(filename,FileMode.Create),Encoding.Default))
                    {
                        str.Write(textBox1.Text);
                        str.Close();
                    }
                }
                catch
                {
                    MessageBox.Show("Ошибка сохранения файла");
                }
            }
        }
        private void button1_Click(object sender, EventArgs e)          //кнопка "Выбрать файл"
        {
            openFileDialog1.FileName = "";                              //пользователь выбирает файл, из которого
            if (openFileDialog1.ShowDialog() == DialogResult.OK)        //требуется считать данные
                filename = openFileDialog1.FileName;
            try
            {
                System.IO.StreamReader stream = new System.IO.StreamReader(filename,Encoding.Default);
                textBox1.Text = stream.ReadToEnd();                     //данные из файла записываются в textBox1
                radioButton1.Visible = true;
                radioButton2.Visible = true;
                button2.Visible = true;
                button3.Visible = true;
                stream.Close();
            }
            catch
            {
                MessageBox.Show("Ошибка чтения файла");
            }
        }
        bool isPrime(int num)                   //функция проверки числа num на простоту
        {
            bool m = true;                      //проверяется путём проверки всех делителей от 2 до num/2
            for (int i = num / 2; i > 1; i--)   //на отсутствие остатка, если его нет хоть в одном случае,
                if (num % i == 0)               //число не является простым
                    m = false;
            return m;
        }
        private void radioButton2_Click(object sender, EventArgs e)
        {
            label2.Visible = true;      //визуальное отображение интерфейса формы
            textBox3.Visible = true;    //отображается не раньше, чем нужно
        }
        private void radioButton1_Click(object sender, EventArgs e)
        {
            label2.Visible = false;     //скрывает ненужный интерфейс для выбранного пункта
            textBox3.Visible = false;
            textBox3.Text = "";
        }
        bool isCoprime(int v1, int v2)  //функция проверки чисел на взаимную простоту
        {                               //используется следующее свойство взаимно простых чисел:
            while (v1 != 0 && v2 != 0)  //числа взаимно простые, если их наибольший
            {                           //общий делитель равен одному
                if (v1 > v2)            //находим НОД по алгоритму Эвклида
                    v1 %= v2;           //и далее проверяем, равен ли он 1
                else
                    v2 %= v1;
            }
            return Math.Max(v1, v2) == 1;
        }
        int modInverse(int a, int n)    //функция для нахождения обратного по модулю числа
        {
            int i = n, v = 0, d = 1;
            while (a > 0)
            {
                int t = i / a, x = a;
                a = i % x;
                i = x;
                x = d;
                d = v - t * x;
                v = x;
            }
            v %= n;
            if (v < 0) v = (v + n) % n;
            return v;
        }
    }
}