namespace Coginov
{
    partial class SimpleTest
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnTest = new System.Windows.Forms.Button();
            this.listElements = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // btnTest
            // 
            this.btnTest.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btnTest.Location = new System.Drawing.Point(0, 238);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(256, 23);
            this.btnTest.TabIndex = 0;
            this.btnTest.Text = "Test Document";
            this.btnTest.UseVisualStyleBackColor = true;
            this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // listElements
            // 
            this.listElements.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listElements.FormattingEnabled = true;
            this.listElements.Location = new System.Drawing.Point(0, 0);
            this.listElements.Name = "listElements";
            this.listElements.Size = new System.Drawing.Size(256, 238);
            this.listElements.TabIndex = 1;
            // 
            // SimpleTest
            // 
            this.AcceptButton = this.btnTest;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(256, 261);
            this.Controls.Add(this.listElements);
            this.Controls.Add(this.btnTest);
            this.Name = "SimpleTest";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnTest;
        private System.Windows.Forms.ListBox listElements;
    }
}

