using System.Text;
using System.Numerics;
namespace kp
{
    public partial class Form1 : Form
    {                           //������� ��� ����������
        char[] alphabet = new char[] { '1','2','3','4','5','6','7','8','9','0','-','+','!','@','#','$','%','^','&','*','(',')','_','=','q','w','e','r','t','y','u','i','o','p','[',']'
        ,'a','s','d','f','g','h','j','k','l',';',' ','z','x','c','v','b','n','m',',','.','/','Q','W','E','R','T','Y','U','I','O','P','{','}','A','S','D','F','G','H','J','K','L'
        ,':','"','\\','\'','|','Z','X','C','V','B','N','M','<','>','?','�','�','�','�','�','�','�','�','�','�','�','�','�','�','�','�','�','�','�','�','�','�','�','�','�','�','�',
        '�','�','�','�','�','/','�','�','�','�','�','�','�','�','�','�','�','�','�','�','�','�','�','�','�','�','�','�','�','�','�','�','�','�','�','�','�','�','�','�','~','`','�','\n','\r','\u2386'};
        int p, q;               //������� �����
        string filename = "";   //�������� �����
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

        private void button2_Click(object sender, EventArgs e) //������ "���������� ������"
        {
            Random rnd = new Random();
            if (radioButton1.Checked)                //���� �������� �����
            {
                p = rnd.Next(20, 50);                //���������� ���������
                q = rnd.Next(50, 100);               //����� p � q, ����������� ��� ����������
                while (!isPrime(p))                  //� ��������� �� �� ������� ��������
                    p++;
                while (!isPrime(q))                  //���� ��� �� �������, ���������� 1 � ��������� �����
                    q++;
                int n = p * q;                       //��������� ������������ ������� �����
                int eul = (p - 1) * (q - 1);         //��������� ������� ������
                int ex = rnd.Next(3, 100);           //������� �������� ����������
                while (!isCoprime(ex, eul))          //��������� � �� �������� ��������
                    ex = rnd.Next(3, 100);           //�� ��������� ������� ������
                int secret = modInverse(ex, eul);    //��������� �������� ����������
                string m = "";                       //������ ������, � ������� ��������� ���������� textBox1
                for (int i = 0; i < textBox1.Lines.Length; i++)
                    m += textBox1.Lines[i].ToString() + " ";
                byte[] data = new byte[m.Length];    //������ ������ 8-������ ������������� ��������, �.�. �������
                textBox1.Text = "";                  //�� ��������� 256 ��������, � ������
                for (int i = 0; i < m.Length; i++)   //����������� ���������� ��� ������ � ���������� textBox1
                {
                    data[i] = Convert.ToByte(Array.IndexOf(alphabet, m[i])); //����� �������� ������� ���������
                    BigInteger raw = new BigInteger(data[i]);                //�������� ������ RSA � ��������� � textBox1
                    BigInteger ciphered = (BigInteger.Pow(raw, ex)) % n;     //BigInteger ������������, ����� �������� ����������
                    textBox1.Text += ciphered.ToString() + " ";              //������� �������, ��� ��������� P � Q ����� ����������� 
                }                                                            //������������� ��� �������������
                label1.Visible = true;
                textBox2.Visible = true;
                textBox2.Text = ex.ToString() + " " + n.ToString();          //� textBox2 ��������� �������� ���� RSA
                MessageBox.Show("�������� ���� ��� ����� ������ ����������: " + secret.ToString() + " " + n.ToString() + ", ��������� ��� �������� ��� ��� ����������� ����������� ������� �����!", "��������", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            if (radioButton2.Checked)           //���� ���������� �����
            {
                try
                {
                    string[] SecrAndN = textBox3.Text.Split(' ');           //����������� �������� ����, �������� �������������
                    int encsecret = Convert.ToInt32(SecrAndN[0].Trim());
                    int encn = Convert.ToInt32(SecrAndN[1].Trim());
                    string[] m = textBox1.Text.Split(' ');                  //����������� ���������� textBox1 - ������������� �����
                    byte[] data = new byte[m.Length - 1];                   //�������� ������ 8-������ ����� ����� ��� �����������
                    for (int i = 0; i < m.Length - 1; i++)                  //� ����� ����� ����������� �� ��������� ����� RSA
                    {
                        Int16 ciphered = Int16.Parse(m[i]);                 //������������ BigInteger �� ��� �� �������, ��� � ����:
                        BigInteger mmmm = (BigInteger.Pow(ciphered, encsecret)) % encn; //�� � ���� �� ����������� ������������� �����
                        byte raw = (byte)mmmm;                              //�� ��������� ������� �������
                        data[i] = Convert.ToByte(raw);
                    }
                    string res = "";                                        //����� ���������� �������� ������, � �������
                    for (int i = 0; i < data.Length; i++)                   //������������ �� �������� ���������� �������
                        res += alphabet[data[i]];                           //8-������ �����, �������������� ������
                    textBox1.Text = res;                                    //������������ � textBox1
                }
                catch
                {
                    MessageBox.Show("������ ������������� ������","��������!",MessageBoxButtons.OK,MessageBoxIcon.Error);
                }
            }
        }
        private void button3_Click(object sender, EventArgs e)          //������ "��������� ���"
        {
            {                                                           //���� ��� ���������� ���������� �������������
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)    //����� ���������� ����
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
                    MessageBox.Show("������ ���������� �����");
                }
            }
        }
        private void button1_Click(object sender, EventArgs e)          //������ "������� ����"
        {
            openFileDialog1.FileName = "";                              //������������ �������� ����, �� ��������
            if (openFileDialog1.ShowDialog() == DialogResult.OK)        //��������� ������� ������
                filename = openFileDialog1.FileName;
            try
            {
                System.IO.StreamReader stream = new System.IO.StreamReader(filename,Encoding.Default);
                textBox1.Text = stream.ReadToEnd();                     //������ �� ����� ������������ � textBox1
                radioButton1.Visible = true;
                radioButton2.Visible = true;
                button2.Visible = true;
                button3.Visible = true;
                stream.Close();
            }
            catch
            {
                MessageBox.Show("������ ������ �����");
            }
        }
        bool isPrime(int num)                   //������� �������� ����� num �� ��������
        {
            bool m = true;                      //����������� ���� �������� ���� ��������� �� 2 �� num/2
            for (int i = num / 2; i > 1; i--)   //�� ���������� �������, ���� ��� ��� ���� � ����� ������,
                if (num % i == 0)               //����� �� �������� �������
                    m = false;
            return m;
        }
        private void radioButton2_Click(object sender, EventArgs e)
        {
            label2.Visible = true;      //���������� ����������� ���������� �����
            textBox3.Visible = true;    //������������ �� ������, ��� �����
        }
        private void radioButton1_Click(object sender, EventArgs e)
        {
            label2.Visible = false;     //�������� �������� ��������� ��� ���������� ������
            textBox3.Visible = false;
            textBox3.Text = "";
        }
        bool isCoprime(int v1, int v2)  //������� �������� ����� �� �������� ��������
        {                               //������������ ��������� �������� ������� ������� �����:
            while (v1 != 0 && v2 != 0)  //����� ������� �������, ���� �� ����������
            {                           //����� �������� ����� ������
                if (v1 > v2)            //������� ��� �� ��������� �������
                    v1 %= v2;           //� ����� ���������, ����� �� �� 1
                else
                    v2 %= v1;
            }
            return Math.Max(v1, v2) == 1;
        }
        int modInverse(int a, int n)    //������� ��� ���������� ��������� �� ������ �����
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