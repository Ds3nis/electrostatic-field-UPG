﻿using System.Drawing;
using System.Windows.Forms;

namespace UPG_SP_2024
{
    partial class MainForm
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
     
            SuspendLayout();
            // 
            // drawingPanel
            // 
            drawingPanel.Dock = DockStyle.Fill;
            drawingPanel.Location = new Point(0, 0);
            drawingPanel.Name = "drawingPanel";
            drawingPanel.Size = new Size(784, 559);
            drawingPanel.TabIndex = 0;
            drawingPanel.Paint += drawingPanel_Paint;
            drawingPanel.Resize += drawingPanel_Resize;
            drawingPanel.ParentChanged += drawingPanel_ParentChanged;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(784, 559);
            Controls.Add(drawingPanel);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "<A23B0083P> - Semestrální práce KIV/UPG 2024/2025";
            ResumeLayout(false);
        }

        #endregion

        private DrawingPanel drawingPanel;
    }
}
