namespace Poker
{
    partial class GameForm
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
            this.buttonFold = new System.Windows.Forms.Button();
            this.buttonCheck = new System.Windows.Forms.Button();
            this.buttonCall = new System.Windows.Forms.Button();
            this.buttonRaise = new System.Windows.Forms.Button();
            this.progressBarTimer = new System.Windows.Forms.ProgressBar();
            this.textBoxPlayerChips = new System.Windows.Forms.TextBox();
            this.buttonAddChips = new System.Windows.Forms.Button();
            this.textBoxAddChips = new System.Windows.Forms.TextBox();
            this.textBoxBot5Chips = new System.Windows.Forms.TextBox();
            this.textBoxBot4Chips = new System.Windows.Forms.TextBox();
            this.textBoxBot3Chips = new System.Windows.Forms.TextBox();
            this.textBoxBot2Chips = new System.Windows.Forms.TextBox();
            this.textBoxBot1Chips = new System.Windows.Forms.TextBox();
            this.textBoxPot = new System.Windows.Forms.TextBox();
            this.buttonChooseBlind = new System.Windows.Forms.Button();
            this.buttonBigBlind = new System.Windows.Forms.Button();
            this.textBoxSmallBlind = new System.Windows.Forms.TextBox();
            this.buttonSmallBlind = new System.Windows.Forms.Button();
            this.textBoxBigBlind = new System.Windows.Forms.TextBox();
            this.labelBot5Status = new System.Windows.Forms.Label();
            this.labelBot4Status = new System.Windows.Forms.Label();
            this.labelBot3Status = new System.Windows.Forms.Label();
            this.labelBot1Status = new System.Windows.Forms.Label();
            this.labelPlayerStatus = new System.Windows.Forms.Label();
            this.labelBot2Status = new System.Windows.Forms.Label();
            this.labelPot = new System.Windows.Forms.Label();
            this.textBoxRaise = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // buttonFold
            // 
            this.buttonFold.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonFold.Font = new System.Drawing.Font("Microsoft Sans Serif", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.buttonFold.Location = new System.Drawing.Point(302, 660);
            this.buttonFold.Name = "buttonFold";
            this.buttonFold.Size = new System.Drawing.Size(130, 62);
            this.buttonFold.TabIndex = 0;
            this.buttonFold.Text = "Fold";
            this.buttonFold.UseVisualStyleBackColor = true;
            // 
            // buttonCheck
            // 
            this.buttonCheck.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonCheck.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.buttonCheck.Location = new System.Drawing.Point(461, 660);
            this.buttonCheck.Name = "buttonCheck";
            this.buttonCheck.Size = new System.Drawing.Size(134, 62);
            this.buttonCheck.TabIndex = 2;
            this.buttonCheck.Text = "Check";
            this.buttonCheck.UseVisualStyleBackColor = true;
            this.buttonCheck.Click += new System.EventHandler(this.ButtonCheck_Click);
            // 
            // buttonCall
            // 
            this.buttonCall.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonCall.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.buttonCall.Location = new System.Drawing.Point(634, 661);
            this.buttonCall.Name = "buttonCall";
            this.buttonCall.Size = new System.Drawing.Size(126, 62);
            this.buttonCall.TabIndex = 3;
            this.buttonCall.Text = "Call";
            this.buttonCall.UseVisualStyleBackColor = true;
            this.buttonCall.Click += new System.EventHandler(this.ButtonCall_Click);
            // 
            // buttonRaise
            // 
            this.buttonRaise.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonRaise.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.buttonRaise.Location = new System.Drawing.Point(802, 661);
            this.buttonRaise.Name = "buttonRaise";
            this.buttonRaise.Size = new System.Drawing.Size(124, 62);
            this.buttonRaise.TabIndex = 4;
            this.buttonRaise.Text = "Raise";
            this.buttonRaise.UseVisualStyleBackColor = true;
            this.buttonRaise.Click += new System.EventHandler(this.ButtonRaise_Click);
            // 
            // progressBarTimer
            // 
            this.progressBarTimer.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.progressBarTimer.BackColor = System.Drawing.SystemColors.Control;
            this.progressBarTimer.Location = new System.Drawing.Point(302, 631);
            this.progressBarTimer.Maximum = 1000;
            this.progressBarTimer.Name = "progressBarTimer";
            this.progressBarTimer.Size = new System.Drawing.Size(667, 23);
            this.progressBarTimer.TabIndex = 5;
            this.progressBarTimer.Value = 1000;
            // 
            // textBoxPlayerChips
            // 
            this.textBoxPlayerChips.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.textBoxPlayerChips.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textBoxPlayerChips.Location = new System.Drawing.Point(722, 553);
            this.textBoxPlayerChips.Name = "textBoxPlayerChips";
            this.textBoxPlayerChips.Size = new System.Drawing.Size(163, 23);
            this.textBoxPlayerChips.TabIndex = 6;
            this.textBoxPlayerChips.Text = "Chips : 0";
            // 
            // buttonAddChips
            // 
            this.buttonAddChips.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonAddChips.Location = new System.Drawing.Point(12, 697);
            this.buttonAddChips.Name = "buttonAddChips";
            this.buttonAddChips.Size = new System.Drawing.Size(75, 25);
            this.buttonAddChips.TabIndex = 7;
            this.buttonAddChips.Text = "AddChips";
            this.buttonAddChips.UseVisualStyleBackColor = true;
            this.buttonAddChips.Click += new System.EventHandler(this.ButtonAddChips_Click);
            // 
            // textBoxAddChips
            // 
            this.textBoxAddChips.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.textBoxAddChips.Location = new System.Drawing.Point(93, 700);
            this.textBoxAddChips.Name = "textBoxAddChips";
            this.textBoxAddChips.Size = new System.Drawing.Size(125, 20);
            this.textBoxAddChips.TabIndex = 8;
            // 
            // textBoxBot5Chips
            // 
            this.textBoxBot5Chips.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxBot5Chips.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textBoxBot5Chips.Location = new System.Drawing.Point(946, 553);
            this.textBoxBot5Chips.Name = "textBoxBot5Chips";
            this.textBoxBot5Chips.Size = new System.Drawing.Size(152, 23);
            this.textBoxBot5Chips.TabIndex = 9;
            this.textBoxBot5Chips.Text = "Chips : 0";
            // 
            // textBoxBot4Chips
            // 
            this.textBoxBot4Chips.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxBot4Chips.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textBoxBot4Chips.Location = new System.Drawing.Point(904, 81);
            this.textBoxBot4Chips.Name = "textBoxBot4Chips";
            this.textBoxBot4Chips.Size = new System.Drawing.Size(123, 23);
            this.textBoxBot4Chips.TabIndex = 10;
            this.textBoxBot4Chips.Text = "Chips : 0";
            // 
            // textBoxBot3Chips
            // 
            this.textBoxBot3Chips.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxBot3Chips.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textBoxBot3Chips.Location = new System.Drawing.Point(689, 81);
            this.textBoxBot3Chips.Name = "textBoxBot3Chips";
            this.textBoxBot3Chips.Size = new System.Drawing.Size(125, 23);
            this.textBoxBot3Chips.TabIndex = 11;
            this.textBoxBot3Chips.Text = "Chips : 0";
            // 
            // textBoxBot2Chips
            // 
            this.textBoxBot2Chips.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textBoxBot2Chips.Location = new System.Drawing.Point(276, 81);
            this.textBoxBot2Chips.Name = "textBoxBot2Chips";
            this.textBoxBot2Chips.Size = new System.Drawing.Size(133, 23);
            this.textBoxBot2Chips.TabIndex = 12;
            this.textBoxBot2Chips.Text = "Chips : 0";
            // 
            // textBoxBot1Chips
            // 
            this.textBoxBot1Chips.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.textBoxBot1Chips.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textBoxBot1Chips.Location = new System.Drawing.Point(181, 553);
            this.textBoxBot1Chips.Name = "textBoxBot1Chips";
            this.textBoxBot1Chips.Size = new System.Drawing.Size(142, 23);
            this.textBoxBot1Chips.TabIndex = 13;
            this.textBoxBot1Chips.Text = "Chips : 0";
            // 
            // textBoxPot
            // 
            this.textBoxPot.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.textBoxPot.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textBoxPot.Location = new System.Drawing.Point(573, 212);
            this.textBoxPot.Name = "textBoxPot";
            this.textBoxPot.Size = new System.Drawing.Size(125, 23);
            this.textBoxPot.TabIndex = 14;
            this.textBoxPot.Text = "0";
            // 
            // buttonChooseBlind
            // 
            this.buttonChooseBlind.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.buttonChooseBlind.Location = new System.Drawing.Point(12, 12);
            this.buttonChooseBlind.Name = "buttonChooseBlind";
            this.buttonChooseBlind.Size = new System.Drawing.Size(127, 29);
            this.buttonChooseBlind.TabIndex = 15;
            this.buttonChooseBlind.Text = "Choose Blind";
            this.buttonChooseBlind.UseVisualStyleBackColor = true;
            this.buttonChooseBlind.Click += new System.EventHandler(this.ButtonChooseBlind_Click);
            // 
            // buttonBigBlind
            // 
            this.buttonBigBlind.Location = new System.Drawing.Point(12, 254);
            this.buttonBigBlind.Name = "buttonBigBlind";
            this.buttonBigBlind.Size = new System.Drawing.Size(75, 23);
            this.buttonBigBlind.TabIndex = 16;
            this.buttonBigBlind.Text = "Big Blind";
            this.buttonBigBlind.UseVisualStyleBackColor = true;
            this.buttonBigBlind.Click += new System.EventHandler(this.ButtonBigBlind_Click);
            // 
            // textBoxSmallBlind
            // 
            this.textBoxSmallBlind.Location = new System.Drawing.Point(12, 228);
            this.textBoxSmallBlind.Name = "textBoxSmallBlind";
            this.textBoxSmallBlind.Size = new System.Drawing.Size(75, 20);
            this.textBoxSmallBlind.TabIndex = 17;
            this.textBoxSmallBlind.Text = "250";
            // 
            // buttonSmallBlind
            // 
            this.buttonSmallBlind.Location = new System.Drawing.Point(12, 199);
            this.buttonSmallBlind.Name = "buttonSmallBlind";
            this.buttonSmallBlind.Size = new System.Drawing.Size(75, 23);
            this.buttonSmallBlind.TabIndex = 18;
            this.buttonSmallBlind.Text = "Small Blind";
            this.buttonSmallBlind.UseVisualStyleBackColor = true;
            this.buttonSmallBlind.Click += new System.EventHandler(this.ButtonSmallBlind_Click);
            // 
            // textBoxBigBlind
            // 
            this.textBoxBigBlind.Location = new System.Drawing.Point(12, 283);
            this.textBoxBigBlind.Name = "textBoxBigBlind";
            this.textBoxBigBlind.Size = new System.Drawing.Size(75, 20);
            this.textBoxBigBlind.TabIndex = 19;
            this.textBoxBigBlind.Text = "500";
            // 
            // labelBot5Status
            // 
            this.labelBot5Status.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.labelBot5Status.Location = new System.Drawing.Point(946, 579);
            this.labelBot5Status.Name = "labelBot5Status";
            this.labelBot5Status.Size = new System.Drawing.Size(152, 32);
            this.labelBot5Status.TabIndex = 26;
            // 
            // labelBot4Status
            // 
            this.labelBot4Status.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelBot4Status.Location = new System.Drawing.Point(904, 107);
            this.labelBot4Status.Name = "labelBot4Status";
            this.labelBot4Status.Size = new System.Drawing.Size(123, 32);
            this.labelBot4Status.TabIndex = 27;
            // 
            // labelBot3Status
            // 
            this.labelBot3Status.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelBot3Status.Location = new System.Drawing.Point(689, 107);
            this.labelBot3Status.Name = "labelBot3Status";
            this.labelBot3Status.Size = new System.Drawing.Size(125, 32);
            this.labelBot3Status.TabIndex = 28;
            // 
            // labelBot1Status
            // 
            this.labelBot1Status.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelBot1Status.Location = new System.Drawing.Point(181, 579);
            this.labelBot1Status.Name = "labelBot1Status";
            this.labelBot1Status.Size = new System.Drawing.Size(142, 32);
            this.labelBot1Status.TabIndex = 29;
            // 
            // labelPlayerStatus
            // 
            this.labelPlayerStatus.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.labelPlayerStatus.Location = new System.Drawing.Point(722, 579);
            this.labelPlayerStatus.Name = "labelPlayerStatus";
            this.labelPlayerStatus.Size = new System.Drawing.Size(163, 32);
            this.labelPlayerStatus.TabIndex = 30;
            // 
            // labelBot2Status
            // 
            this.labelBot2Status.Location = new System.Drawing.Point(276, 107);
            this.labelBot2Status.Name = "labelBot2Status";
            this.labelBot2Status.Size = new System.Drawing.Size(133, 32);
            this.labelBot2Status.TabIndex = 31;
            // 
            // labelPot
            // 
            this.labelPot.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.labelPot.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelPot.Location = new System.Drawing.Point(621, 188);
            this.labelPot.Name = "labelPot";
            this.labelPot.Size = new System.Drawing.Size(31, 21);
            this.labelPot.TabIndex = 0;
            this.labelPot.Text = "Pot";
            // 
            // textBoxRaise
            // 
            this.textBoxRaise.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.textBoxRaise.Location = new System.Drawing.Point(940, 700);
            this.textBoxRaise.Name = "textBoxRaise";
            this.textBoxRaise.Size = new System.Drawing.Size(108, 20);
            this.textBoxRaise.TabIndex = 0;
            // 
            // GameForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::Poker.Properties.Resources.poker_table___Copy;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1284, 729);
            this.Controls.Add(this.textBoxRaise);
            this.Controls.Add(this.labelPot);
            this.Controls.Add(this.labelBot2Status);
            this.Controls.Add(this.labelPlayerStatus);
            this.Controls.Add(this.labelBot1Status);
            this.Controls.Add(this.labelBot3Status);
            this.Controls.Add(this.labelBot4Status);
            this.Controls.Add(this.labelBot5Status);
            this.Controls.Add(this.textBoxBigBlind);
            this.Controls.Add(this.buttonSmallBlind);
            this.Controls.Add(this.textBoxSmallBlind);
            this.Controls.Add(this.buttonBigBlind);
            this.Controls.Add(this.buttonChooseBlind);
            this.Controls.Add(this.textBoxPot);
            this.Controls.Add(this.textBoxBot1Chips);
            this.Controls.Add(this.textBoxBot2Chips);
            this.Controls.Add(this.textBoxBot3Chips);
            this.Controls.Add(this.textBoxBot4Chips);
            this.Controls.Add(this.textBoxBot5Chips);
            this.Controls.Add(this.textBoxAddChips);
            this.Controls.Add(this.buttonAddChips);
            this.Controls.Add(this.textBoxPlayerChips);
            this.Controls.Add(this.progressBarTimer);
            this.Controls.Add(this.buttonRaise);
            this.Controls.Add(this.buttonCall);
            this.Controls.Add(this.buttonCheck);
            this.Controls.Add(this.buttonFold);
            this.DoubleBuffered = true;
            this.Name = "GameForm";
            this.Text = "GLS Texas Poker";
            this.Layout += new System.Windows.Forms.LayoutEventHandler(this.Layout_Change);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonFold;
        private System.Windows.Forms.Button buttonCheck;
        private System.Windows.Forms.Button buttonCall;
        private System.Windows.Forms.Button buttonRaise;
        private System.Windows.Forms.ProgressBar progressBarTimer;
        private System.Windows.Forms.TextBox textBoxPlayerChips;
        private System.Windows.Forms.Button buttonAddChips;
        private System.Windows.Forms.TextBox textBoxAddChips;
        private System.Windows.Forms.TextBox textBoxBot5Chips;
        private System.Windows.Forms.TextBox textBoxBot4Chips;
        private System.Windows.Forms.TextBox textBoxBot3Chips;
        private System.Windows.Forms.TextBox textBoxBot2Chips;
        private System.Windows.Forms.TextBox textBoxBot1Chips;
        private System.Windows.Forms.TextBox textBoxPot;
        private System.Windows.Forms.Button buttonChooseBlind;
        private System.Windows.Forms.Button buttonBigBlind;
        private System.Windows.Forms.TextBox textBoxSmallBlind;
        private System.Windows.Forms.Button buttonSmallBlind;
        private System.Windows.Forms.TextBox textBoxBigBlind;
        private System.Windows.Forms.Label labelBot5Status;
        private System.Windows.Forms.Label labelBot4Status;
        private System.Windows.Forms.Label labelBot3Status;
        private System.Windows.Forms.Label labelBot1Status;
        private System.Windows.Forms.Label labelPlayerStatus;
        private System.Windows.Forms.Label labelBot2Status;
        private System.Windows.Forms.Label labelPot;
        private System.Windows.Forms.TextBox textBoxRaise;



    }
}

