namespace ElectricFieldVis
{
    partial class GraphForm
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
            chart = new LiveChartsCore.SkiaSharpView.WinForms.CartesianChart();
            updateBtn = new Button();
            SuspendLayout();
            // 
            // chart
            // 
            chart.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            chart.Location = new Point(12, 12);
            chart.Name = "chart";
            chart.Size = new Size(776, 385);
            chart.TabIndex = 0;
            // 
            // updateBtn
            // 
            updateBtn.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            updateBtn.Location = new Point(280, 403);
            updateBtn.Name = "updateBtn";
            updateBtn.Size = new Size(242, 35);
            updateBtn.TabIndex = 1;
            updateBtn.Text = "button1";
            updateBtn.UseVisualStyleBackColor = true;
            updateBtn.Click += updateBtn_Click;
            // 
            // GraphForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(updateBtn);
            Controls.Add(chart);
            Name = "GraphForm";
            Text = "GraphForm";
            Load += GraphForm_Load;
            ResumeLayout(false);
        }

        #endregion

        private LiveChartsCore.SkiaSharpView.WinForms.CartesianChart chart;
        private Button updateBtn;
    }
}