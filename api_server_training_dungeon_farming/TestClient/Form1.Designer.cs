namespace TestClient
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            listBox1 = new ListBox();
            button1 = new Button();
            groupBox1 = new GroupBox();
            textBox1 = new TextBox();
            textBoxPW = new TextBox();
            label2 = new Label();
            textBoxID = new TextBox();
            label1 = new Label();
            textBoxAuthToken = new TextBox();
            label3 = new Label();
            textBoxClientVer = new TextBox();
            label4 = new Label();
            textBoxMasterDataVer = new TextBox();
            label5 = new Label();
            groupBox2 = new GroupBox();
            button2 = new Button();
            textBox2 = new TextBox();
            listBox3 = new ListBox();
            groupBox3 = new GroupBox();
            textBox3 = new TextBox();
            label6 = new Label();
            button3 = new Button();
            button4 = new Button();
            textBox4 = new TextBox();
            button5 = new Button();
            button6 = new Button();
            listBox2 = new ListBox();
            button7 = new Button();
            groupBox4 = new GroupBox();
            button8 = new Button();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            groupBox3.SuspendLayout();
            groupBox4.SuspendLayout();
            SuspendLayout();
            // 
            // listBox1
            // 
            listBox1.FormattingEnabled = true;
            listBox1.ItemHeight = 15;
            listBox1.Location = new Point(26, 442);
            listBox1.Name = "listBox1";
            listBox1.Size = new Size(752, 169);
            listBox1.TabIndex = 0;
            listBox1.SelectedIndexChanged += listBox1_SelectedIndexChanged;
            // 
            // button1
            // 
            button1.Location = new Point(293, 21);
            button1.Name = "button1";
            button1.Size = new Size(58, 27);
            button1.TabIndex = 1;
            button1.Text = "요청";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(button1);
            groupBox1.Controls.Add(textBox1);
            groupBox1.Location = new Point(12, 52);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(358, 63);
            groupBox1.TabIndex = 2;
            groupBox1.TabStop = false;
            groupBox1.Text = "계정 생성";
            // 
            // textBox1
            // 
            textBox1.Location = new Point(16, 23);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(274, 23);
            textBox1.TabIndex = 0;
            textBox1.Text = "http://";
            // 
            // textBoxPW
            // 
            textBoxPW.Location = new Point(165, 12);
            textBoxPW.Name = "textBoxPW";
            textBoxPW.Size = new Size(91, 23);
            textBoxPW.TabIndex = 4;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(135, 15);
            label2.Name = "label2";
            label2.Size = new Size(28, 15);
            label2.TabIndex = 5;
            label2.Text = "PW:";
            // 
            // textBoxID
            // 
            textBoxID.Location = new Point(36, 12);
            textBoxID.Name = "textBoxID";
            textBoxID.Size = new Size(93, 23);
            textBoxID.TabIndex = 3;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(11, 15);
            label1.Name = "label1";
            label1.Size = new Size(22, 15);
            label1.TabIndex = 3;
            label1.Text = "ID:";
            // 
            // textBoxAuthToken
            // 
            textBoxAuthToken.Location = new Point(343, 12);
            textBoxAuthToken.Name = "textBoxAuthToken";
            textBoxAuthToken.Size = new Size(100, 23);
            textBoxAuthToken.TabIndex = 6;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(273, 15);
            label3.Name = "label3";
            label3.Size = new Size(68, 15);
            label3.TabIndex = 7;
            label3.Text = "AuthToken:";
            // 
            // textBoxClientVer
            // 
            textBoxClientVer.Location = new Point(520, 12);
            textBoxClientVer.Name = "textBoxClientVer";
            textBoxClientVer.Size = new Size(38, 23);
            textBoxClientVer.TabIndex = 8;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(457, 15);
            label4.Name = "label4";
            label4.Size = new Size(62, 15);
            label4.TabIndex = 9;
            label4.Text = "클라 버전:";
            // 
            // textBoxMasterDataVer
            // 
            textBoxMasterDataVer.Location = new Point(685, 12);
            textBoxMasterDataVer.Name = "textBoxMasterDataVer";
            textBoxMasterDataVer.Size = new Size(38, 23);
            textBoxMasterDataVer.TabIndex = 10;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(574, 16);
            label5.Name = "label5";
            label5.Size = new Size(110, 15);
            label5.TabIndex = 11;
            label5.Text = "마스터데이터 버전:";
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(button2);
            groupBox2.Controls.Add(textBox2);
            groupBox2.Location = new Point(389, 52);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(358, 63);
            groupBox2.TabIndex = 12;
            groupBox2.TabStop = false;
            groupBox2.Text = "로그인";
            // 
            // button2
            // 
            button2.Location = new Point(288, 20);
            button2.Name = "button2";
            button2.Size = new Size(58, 27);
            button2.TabIndex = 1;
            button2.Text = "요청";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // textBox2
            // 
            textBox2.Location = new Point(16, 23);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(266, 23);
            textBox2.TabIndex = 0;
            textBox2.Text = "http://";
            // 
            // listBox3
            // 
            listBox3.FormattingEnabled = true;
            listBox3.ItemHeight = 15;
            listBox3.Location = new Point(6, 22);
            listBox3.Name = "listBox3";
            listBox3.Size = new Size(259, 274);
            listBox3.TabIndex = 14;
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(button6);
            groupBox3.Controls.Add(button5);
            groupBox3.Controls.Add(textBox4);
            groupBox3.Controls.Add(button4);
            groupBox3.Controls.Add(button3);
            groupBox3.Controls.Add(label6);
            groupBox3.Controls.Add(textBox3);
            groupBox3.Controls.Add(listBox3);
            groupBox3.Location = new Point(11, 121);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(339, 315);
            groupBox3.TabIndex = 15;
            groupBox3.TabStop = false;
            groupBox3.Text = "메일 박스";
            // 
            // textBox3
            // 
            textBox3.Location = new Point(271, 48);
            textBox3.Name = "textBox3";
            textBox3.Size = new Size(46, 23);
            textBox3.TabIndex = 16;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(271, 30);
            label6.Name = "label6";
            label6.Size = new Size(46, 15);
            label6.TabIndex = 16;
            label6.Text = "페이지:";
            // 
            // button3
            // 
            button3.Location = new Point(271, 77);
            button3.Name = "button3";
            button3.Size = new Size(46, 27);
            button3.TabIndex = 16;
            button3.Text = ">>";
            button3.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            button4.Location = new Point(271, 127);
            button4.Name = "button4";
            button4.Size = new Size(46, 27);
            button4.TabIndex = 17;
            button4.Text = "읽기";
            button4.UseVisualStyleBackColor = true;
            // 
            // textBox4
            // 
            textBox4.Location = new Point(271, 180);
            textBox4.Name = "textBox4";
            textBox4.Size = new Size(46, 23);
            textBox4.TabIndex = 17;
            // 
            // button5
            // 
            button5.Location = new Point(271, 209);
            button5.Name = "button5";
            button5.Size = new Size(46, 27);
            button5.TabIndex = 18;
            button5.Text = "삭제";
            button5.UseVisualStyleBackColor = true;
            // 
            // button6
            // 
            button6.Location = new Point(271, 242);
            button6.Name = "button6";
            button6.Size = new Size(46, 27);
            button6.TabIndex = 19;
            button6.Text = "받기";
            button6.UseVisualStyleBackColor = true;
            // 
            // listBox2
            // 
            listBox2.FormattingEnabled = true;
            listBox2.ItemHeight = 15;
            listBox2.Location = new Point(6, 21);
            listBox2.Name = "listBox2";
            listBox2.Size = new Size(284, 244);
            listBox2.TabIndex = 16;
            // 
            // button7
            // 
            button7.Location = new Point(699, 130);
            button7.Name = "button7";
            button7.Size = new Size(89, 27);
            button7.TabIndex = 17;
            button7.Text = "출석체크";
            button7.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            groupBox4.Controls.Add(button8);
            groupBox4.Controls.Add(listBox2);
            groupBox4.Location = new Point(362, 130);
            groupBox4.Name = "groupBox4";
            groupBox4.Size = new Size(309, 301);
            groupBox4.TabIndex = 18;
            groupBox4.TabStop = false;
            groupBox4.Text = "groupBox4";
            // 
            // button8
            // 
            button8.Location = new Point(5, 268);
            button8.Name = "button8";
            button8.Size = new Size(89, 27);
            button8.TabIndex = 19;
            button8.Text = "강화";
            button8.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 622);
            Controls.Add(groupBox4);
            Controls.Add(button7);
            Controls.Add(groupBox3);
            Controls.Add(groupBox2);
            Controls.Add(textBoxMasterDataVer);
            Controls.Add(label5);
            Controls.Add(textBoxClientVer);
            Controls.Add(label4);
            Controls.Add(textBoxAuthToken);
            Controls.Add(label3);
            Controls.Add(textBoxPW);
            Controls.Add(groupBox1);
            Controls.Add(label2);
            Controls.Add(listBox1);
            Controls.Add(textBoxID);
            Controls.Add(label1);
            Name = "Form1";
            Text = "Form1";
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            groupBox4.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ListBox listBox1;
        private Button button1;
        private GroupBox groupBox1;
        private TextBox textBoxPW;
        private Label label2;
        private TextBox textBoxID;
        private Label label1;
        private TextBox textBox1;
        private TextBox textBoxAuthToken;
        private Label label3;
        private TextBox textBoxClientVer;
        private Label label4;
        private TextBox textBoxMasterDataVer;
        private Label label5;
        private GroupBox groupBox2;
        private Button button2;
        private TextBox textBox2;
        private ListBox listBox3;
        private GroupBox groupBox3;
        private Button button6;
        private Button button5;
        private TextBox textBox4;
        private Button button4;
        private Button button3;
        private Label label6;
        private TextBox textBox3;
        private ListBox listBox2;
        private Button button7;
        private GroupBox groupBox4;
        private Button button8;
    }
}