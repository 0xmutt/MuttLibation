﻿using DataLayer;
using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace LibationWinForms.GridView
{
	public partial class MyRatingCellEditor : UserControl, IDataGridViewEditingControl
	{
		private const string SOLID_STAR = "★";
		private const string HOLLOW_STAR = "☆";

		private Rating _rating;
		public Rating Rating
		{ 
			get => _rating;
			set
			{
				_rating = value;
				int rating = 0;
				foreach (NoBorderLabel star in panelOverall.Controls)
					star.Tag = star.Text = _rating.OverallRating > rating++ ? SOLID_STAR : HOLLOW_STAR;

				rating = 0;
				foreach (NoBorderLabel star in panelPerform.Controls)
					star.Tag = star.Text = _rating.PerformanceRating > rating++ ? SOLID_STAR : HOLLOW_STAR;

				rating = 0;
				foreach (NoBorderLabel star in panelStory.Controls)
					star.Tag = star.Text = _rating.StoryRating > rating++ ? SOLID_STAR : HOLLOW_STAR;
			}
		}

		public MyRatingCellEditor()
		{
			InitializeComponent();
		}

		private void Star_MouseEnter(object sender, EventArgs e)
		{
			var thisTbox = sender as NoBorderLabel;
			var panel = thisTbox.Parent as Panel;
			var star = SOLID_STAR;

			foreach (NoBorderLabel child in panel.Controls)
			{
				child.Text = star;
				if (child == thisTbox) star = HOLLOW_STAR;
			}
		}

		private void Star_MouseLeave(object sender, EventArgs e)
		{
			var thisTbox = sender as NoBorderLabel;
			var panel = thisTbox.Parent as Panel;

			//Artifically shrink rectangle to guarantee mouse is outside when exiting from the left (negative X)
			var clientPt = panel.PointToClient(MousePosition);
			var rect = new Rectangle(0, 0, panel.ClientRectangle.Width - 2, panel.ClientRectangle.Height);
			if (!rect.Contains(clientPt.X - 2, clientPt.Y))
			{
				//Restore defaults
				foreach (NoBorderLabel child in panel.Controls)
					child.Text = (string)child.Tag;
			}
		}

		private void Star_MouseClick(object sender, MouseEventArgs e)
		{
			var overall = Rating.OverallRating;
			var perform = Rating.PerformanceRating;
			var story = Rating.StoryRating;

			var thisTbox = sender as NoBorderLabel;
			var panel = thisTbox.Parent as Panel;

			int newRatingValue = 0;
			foreach (var child in panel.Controls)
			{
				newRatingValue++;
				if (child == thisTbox) break;
			}

			if (panel == panelOverall)
				overall = newRatingValue;
			else if (panel == panelPerform)
				perform = newRatingValue;
			else if (panel == panelStory)
				story = newRatingValue;

			if (overall + perform + story == 0f) return;

			var newRating = new Rating(overall, perform, story);

			if (newRating == Rating) return;

			Rating = newRating;
			EditingControlValueChanged = true;
			EditingControlDataGridView.NotifyCurrentCellDirty(true);
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Escape)
			{
				EditingControlDataGridView.RefreshEdit();
				EditingControlDataGridView.CancelEdit();
				EditingControlDataGridView.CurrentCell.DetachEditingControl();
				EditingControlDataGridView.CurrentCell = null;

			}
			base.OnKeyDown(e);
		}

		#region IDataGridViewEditingControl

		public DataGridView EditingControlDataGridView { get; set; }
		public int EditingControlRowIndex { get; set; }
		public bool EditingControlValueChanged { get; set; }
		public object EditingControlFormattedValue { get => Rating; set { } }
		public Cursor EditingPanelCursor => Cursor;
		public bool RepositionEditingControlOnValueChange => false;

		public void ApplyCellStyleToEditingControl(DataGridViewCellStyle dataGridViewCellStyle)
		{
			Font = dataGridViewCellStyle.Font;
			ForeColor = dataGridViewCellStyle.ForeColor;
			BackColor = dataGridViewCellStyle.BackColor;
		}

		public bool EditingControlWantsInputKey(Keys keyData, bool dataGridViewWantsInputKey) => keyData == Keys.Escape;
		public object GetEditingControlFormattedValue(DataGridViewDataErrorContexts context) => EditingControlFormattedValue;
		public void PrepareEditingControlForEdit(bool selectAll) { }

		#endregion
	}

	public class NoBorderLabel : Panel
	{
		private string _text;
		[Description("Label text"), Category("Data")]
		[Browsable(true)]
		[EditorBrowsable(EditorBrowsableState.Always)]
		[AllowNull]
		public override string Text
		{
			get => _text;
			set
			{
				_text = value;
				Invalidate();
			}
		}

		[Description("X and Y offset for text drawing position. May be negative."), Category("Layout")]
		[Browsable(true)]
		[EditorBrowsable(EditorBrowsableState.Always)]
		public Point LabelOffset { get; set; }
		protected override void OnPaint(PaintEventArgs e)
		{
			TextRenderer.DrawText(e, Text, this.Font, LabelOffset, this.ForeColor);
			base.OnPaint(e);
		}
	}
}
