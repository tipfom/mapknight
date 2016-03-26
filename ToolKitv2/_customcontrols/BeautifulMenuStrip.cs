using System;
using System.Drawing;
using System.Windows.Forms;

namespace mapKnight.ToolKit {
    public class BeautifulMenuStrip : MenuStrip {
        public BeautifulMenuStrip () : base () {
            this.Font = Properties.Settings.Default.MenuStripFont;
            this.BackColor = Properties.Settings.Default.BackColor;
            this.RenderMode = ToolStripRenderMode.ManagerRenderMode;
            this.Renderer = new BeautifulMenuStripRenderer ();
        }
        
        public class BeautifulMenuStripRenderer : ToolStripProfessionalRenderer {
            public const int OUTLINE_HEIGHT = 2;

            private static Brush activeForeBrush = new SolidBrush (Properties.Settings.Default.MenuStripActiveForeColor);
            private static Brush activeBackBrush = new SolidBrush (Properties.Settings.Default.MenuStripActiveBackColor);
            private static Brush inActiveForeBrush = new SolidBrush (Properties.Settings.Default.MenuStripInActiveForeColor);
            private static Brush inActiveBackBrush = new SolidBrush (Properties.Settings.Default.MenuStripInActiveBackColor);
            private static Brush outlineBrush = new SolidBrush (Properties.Settings.Default.MenuStripOutlineColor);
            private static Brush activeDropDownForeBrush = new SolidBrush (Properties.Settings.Default.MenuStripActiveDropDownForeColor);
            private static Brush activeDropDownBackBrush = new SolidBrush (Properties.Settings.Default.MenuStripActiveDropDownBackColor);
            private static Brush inActiveDropDownForeBrush = new SolidBrush (Properties.Settings.Default.MenuStripInActiveDropDownForeColor);
            private static Brush inActiveDropDownBackBrush = new SolidBrush (Properties.Settings.Default.MenuStripInActiveDropDownBackColor);
            private static Brush dropDownOutlineBrush = new SolidBrush (Properties.Settings.Default.MenuStripDropDownOutlineColor);
            private static Brush checkedForeBrush = new SolidBrush (Properties.Settings.Default.MenuStripCheckedForeColor);
            private static Brush checkedBackBrush = new SolidBrush (Properties.Settings.Default.MenuStripCheckedBackColor);
            private static Brush checkedOutlineBrush = new SolidBrush (Properties.Settings.Default.MenuStripCheckedOutlineColor);

            private static StringFormat format = new StringFormat () { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };

            protected override void OnRenderItemBackground (ToolStripItemRenderEventArgs e) {
                // base.OnRenderItemBackground (e);
            }

            protected override void OnRenderButtonBackground (ToolStripItemRenderEventArgs e) {
                //base.OnRenderButtonBackground (e);
            }
            protected override void OnRenderImageMargin (ToolStripRenderEventArgs e) {
                //base.OnRenderImageMargin (e);
            }
            protected override void OnRenderItemImage (ToolStripItemImageRenderEventArgs e) {
                //base.OnRenderItemImage (e);
            }
            protected override void OnRenderToolStripBorder (ToolStripRenderEventArgs e) {
                //base.OnRenderToolStripBorder (e);

            }
            protected override void OnRenderMenuItemBackground (ToolStripItemRenderEventArgs e) {
                if (e.Item.IsOnDropDown) {
                    if (e.Item.Selected) {
                        e.Graphics.FillRectangle (activeDropDownBackBrush, e.Item.ContentRectangle);

                        if (e.Item.Image != null) {
                            switch (e.Item.DisplayStyle) {
                            case ToolStripItemDisplayStyle.Image:
                                e.Graphics.DrawImage (e.Item.Image, e.Item.ContentRectangle.X, e.Item.ContentRectangle.Y, e.Item.ContentRectangle.Height, e.Item.ContentRectangle.Height);
                                break;
                            case ToolStripItemDisplayStyle.ImageAndText:
                                e.Graphics.DrawImage (e.Item.Image, e.Item.ContentRectangle.X, e.Item.ContentRectangle.Y, e.Item.ContentRectangle.Height, e.Item.ContentRectangle.Height);
                                e.Graphics.DrawString (e.Item.Text, e.Item.Owner.Font, activeDropDownForeBrush, new Rectangle (e.Item.ContentRectangle.X + e.Item.ContentRectangle.Height, e.Item.ContentRectangle.Y, e.Item.ContentRectangle.Width - e.Item.ContentRectangle.Height, e.Item.ContentRectangle.Height), format);
                                break;
                            case ToolStripItemDisplayStyle.Text:
                                e.Graphics.DrawString (e.Item.Text, e.Item.Owner.Font, activeDropDownForeBrush, e.Item.ContentRectangle, format);
                                break;
                            }
                        } else {
                            e.Graphics.DrawString (e.Item.Text, e.Item.Owner.Font, activeDropDownForeBrush, e.Item.ContentRectangle, format);
                        }

                        e.Graphics.FillRectangle (dropDownOutlineBrush, 0, 0, e.Item.Width, OUTLINE_HEIGHT);
                        e.Graphics.FillRectangle (dropDownOutlineBrush, 0, 0 + e.Item.Height - OUTLINE_HEIGHT, e.Item.Width, OUTLINE_HEIGHT);
                        e.Graphics.FillRectangle (dropDownOutlineBrush, 0, 0, OUTLINE_HEIGHT, e.Item.Height);
                        e.Graphics.FillRectangle (dropDownOutlineBrush, e.Item.Width - OUTLINE_HEIGHT, 0, OUTLINE_HEIGHT, e.Item.Height);
                        //draw outline
                    } else if (((ToolStripMenuItem)e.Item).Checked) {
                        e.Graphics.FillRectangle (checkedBackBrush, e.Item.ContentRectangle);

                        if (e.Item.Image != null) {
                            switch (e.Item.DisplayStyle) {
                            case ToolStripItemDisplayStyle.Image:
                                e.Graphics.DrawImage (e.Item.Image, e.Item.ContentRectangle.X, e.Item.ContentRectangle.Y, e.Item.ContentRectangle.Height, e.Item.ContentRectangle.Height);
                                break;
                            case ToolStripItemDisplayStyle.ImageAndText:
                                e.Graphics.DrawImage (e.Item.Image, e.Item.ContentRectangle.X, e.Item.ContentRectangle.Y, e.Item.ContentRectangle.Height, e.Item.ContentRectangle.Height);
                                e.Graphics.DrawString (e.Item.Text, e.Item.Owner.Font, checkedForeBrush, new Rectangle (e.Item.ContentRectangle.X + e.Item.ContentRectangle.Height, e.Item.ContentRectangle.Y, e.Item.ContentRectangle.Width - e.Item.ContentRectangle.Height, e.Item.ContentRectangle.Height), format);
                                break;
                            case ToolStripItemDisplayStyle.Text:
                                e.Graphics.DrawString (e.Item.Text, e.Item.Owner.Font, checkedForeBrush, e.Item.ContentRectangle, format);
                                break;
                            }
                        } else {
                            e.Graphics.DrawString (e.Item.Text, e.Item.Owner.Font, checkedForeBrush, e.Item.ContentRectangle, format);
                        }

                        e.Graphics.FillRectangle (checkedOutlineBrush, 0, 0, e.Item.Width, OUTLINE_HEIGHT);
                        e.Graphics.FillRectangle (checkedOutlineBrush, 0, 0 + e.Item.Height - OUTLINE_HEIGHT, e.Item.Width, OUTLINE_HEIGHT);
                        e.Graphics.FillRectangle (checkedOutlineBrush, 0, 0, OUTLINE_HEIGHT, e.Item.Height);
                        e.Graphics.FillRectangle (checkedOutlineBrush, e.Item.Width - OUTLINE_HEIGHT, 0, OUTLINE_HEIGHT, e.Item.Height);
                        //draw outline
                    } else {
                        e.Graphics.FillRectangle (inActiveDropDownBackBrush, e.Item.ContentRectangle);

                        if (e.Item.Image != null) {
                            switch (e.Item.DisplayStyle) {
                            case ToolStripItemDisplayStyle.Image:
                                e.Graphics.DrawImage (e.Item.Image, e.Item.ContentRectangle.X, e.Item.ContentRectangle.Y, e.Item.ContentRectangle.Height, e.Item.ContentRectangle.Height);
                                break;
                            case ToolStripItemDisplayStyle.ImageAndText:
                                e.Graphics.DrawImage (e.Item.Image, e.Item.ContentRectangle.X, e.Item.ContentRectangle.Y, e.Item.ContentRectangle.Height, e.Item.ContentRectangle.Height);
                                e.Graphics.DrawString (e.Item.Text, e.Item.Owner.Font, inActiveDropDownForeBrush, new Rectangle (e.Item.ContentRectangle.X + e.Item.ContentRectangle.Height, e.Item.ContentRectangle.Y, e.Item.ContentRectangle.Width - e.Item.ContentRectangle.Height, e.Item.ContentRectangle.Height), format);
                                break;
                            case ToolStripItemDisplayStyle.Text:
                                e.Graphics.DrawString (e.Item.Text, e.Item.Owner.Font, inActiveDropDownForeBrush, e.Item.ContentRectangle, format);
                                break;
                            }
                        } else {
                            e.Graphics.DrawString (e.Item.Text, e.Item.Owner.Font, inActiveDropDownForeBrush, e.Item.ContentRectangle, format);
                        }
                    }
                } else {
                    if (e.Item.Selected) {
                        e.Graphics.FillRectangle (activeBackBrush, e.Item.ContentRectangle);

                        if (e.Item.Image != null) {
                            switch (e.Item.DisplayStyle) {
                            case ToolStripItemDisplayStyle.Image:
                                e.Graphics.DrawImage (e.Item.Image, e.Item.ContentRectangle.X, e.Item.ContentRectangle.Y, e.Item.ContentRectangle.Height, e.Item.ContentRectangle.Height);
                                break;
                            case ToolStripItemDisplayStyle.ImageAndText:
                                e.Graphics.DrawImage (e.Item.Image, e.Item.ContentRectangle.X, e.Item.ContentRectangle.Y, e.Item.ContentRectangle.Height, e.Item.ContentRectangle.Height);
                                e.Graphics.DrawString (e.Item.Text, e.Item.Owner.Font, activeForeBrush, new Rectangle (e.Item.ContentRectangle.X + e.Item.ContentRectangle.Height, e.Item.ContentRectangle.Y, e.Item.ContentRectangle.Width - e.Item.ContentRectangle.Height, e.Item.ContentRectangle.Height), format);
                                break;
                            case ToolStripItemDisplayStyle.Text:
                                e.Graphics.DrawString (e.Item.Text, e.Item.Owner.Font, activeForeBrush, e.Item.ContentRectangle, format);
                                break;
                            }
                        } else {
                            e.Graphics.DrawString (e.Item.Text, e.Item.Owner.Font, activeForeBrush, e.Item.ContentRectangle, format);
                        }

                        e.Graphics.FillRectangle (outlineBrush, 0, 0, e.Item.Width, OUTLINE_HEIGHT);
                        e.Graphics.FillRectangle (outlineBrush, 0, 0 + e.Item.Height - OUTLINE_HEIGHT, e.Item.Width, OUTLINE_HEIGHT);
                        e.Graphics.FillRectangle (outlineBrush, 0, 0, OUTLINE_HEIGHT, e.Item.Height);
                        e.Graphics.FillRectangle (outlineBrush, e.Item.Width - OUTLINE_HEIGHT, 0, OUTLINE_HEIGHT, e.Item.Height);
                        //draw outline
                    } else if (((ToolStripMenuItem)e.Item).Checked) {
                        e.Graphics.FillRectangle (checkedBackBrush, e.Item.ContentRectangle);

                        if (e.Item.Image != null) {
                            switch (e.Item.DisplayStyle) {
                            case ToolStripItemDisplayStyle.Image:
                                e.Graphics.DrawImage (e.Item.Image, e.Item.ContentRectangle.X, e.Item.ContentRectangle.Y, e.Item.ContentRectangle.Height, e.Item.ContentRectangle.Height);
                                break;
                            case ToolStripItemDisplayStyle.ImageAndText:
                                e.Graphics.DrawImage (e.Item.Image, e.Item.ContentRectangle.X, e.Item.ContentRectangle.Y, e.Item.ContentRectangle.Height, e.Item.ContentRectangle.Height);
                                e.Graphics.DrawString (e.Item.Text, e.Item.Owner.Font, checkedForeBrush, new Rectangle (e.Item.ContentRectangle.X + e.Item.ContentRectangle.Height, e.Item.ContentRectangle.Y, e.Item.ContentRectangle.Width - e.Item.ContentRectangle.Height, e.Item.ContentRectangle.Height), format);
                                break;
                            case ToolStripItemDisplayStyle.Text:
                                e.Graphics.DrawString (e.Item.Text, e.Item.Owner.Font, checkedForeBrush, e.Item.ContentRectangle, format);
                                break;
                            }
                        } else {
                            e.Graphics.DrawString (e.Item.Text, e.Item.Owner.Font, checkedForeBrush, e.Item.ContentRectangle, format);
                        }

                        e.Graphics.FillRectangle (checkedOutlineBrush, 0, 0, e.Item.Width, OUTLINE_HEIGHT);
                        e.Graphics.FillRectangle (checkedOutlineBrush, 0, 0 + e.Item.Height - OUTLINE_HEIGHT, e.Item.Width, OUTLINE_HEIGHT);
                        e.Graphics.FillRectangle (checkedOutlineBrush, 0, 0, OUTLINE_HEIGHT, e.Item.Height);
                        e.Graphics.FillRectangle (checkedOutlineBrush, e.Item.Width - OUTLINE_HEIGHT, 0, OUTLINE_HEIGHT, e.Item.Height);
                        //draw outline
                    } else {
                        e.Graphics.FillRectangle (inActiveBackBrush, e.Item.ContentRectangle);

                        if (e.Item.Image != null) {
                            switch (e.Item.DisplayStyle) {
                            case ToolStripItemDisplayStyle.Image:
                                e.Graphics.DrawImage (e.Item.Image, e.Item.ContentRectangle.X, e.Item.ContentRectangle.Y, e.Item.ContentRectangle.Height, e.Item.ContentRectangle.Height);
                                break;
                            case ToolStripItemDisplayStyle.ImageAndText:
                                e.Graphics.DrawImage (e.Item.Image, e.Item.ContentRectangle.X, e.Item.ContentRectangle.Y, e.Item.ContentRectangle.Height, e.Item.ContentRectangle.Height);
                                e.Graphics.DrawString (e.Item.Text, e.Item.Owner.Font, inActiveForeBrush, new Rectangle (e.Item.ContentRectangle.X + e.Item.ContentRectangle.Height, e.Item.ContentRectangle.Y, e.Item.ContentRectangle.Width - e.Item.ContentRectangle.Height, e.Item.ContentRectangle.Height), format);
                                break;
                            case ToolStripItemDisplayStyle.Text:
                                e.Graphics.DrawString (e.Item.Text, e.Item.Owner.Font, inActiveForeBrush, e.Item.ContentRectangle, format);
                                break;
                            }
                        } else {
                            e.Graphics.DrawString (e.Item.Text, e.Item.Owner.Font, inActiveForeBrush, e.Item.ContentRectangle, format);
                        }
                    }
                }
            }

            protected override void OnRenderArrow (ToolStripArrowRenderEventArgs e) {
                base.OnRenderArrow (e);
            }
            
            protected override void OnRenderDropDownButtonBackground (ToolStripItemRenderEventArgs e) {
                base.OnRenderDropDownButtonBackground (e);
            }

            protected override void OnRenderGrip (ToolStripGripRenderEventArgs e) {
                base.OnRenderGrip (e);
            }
            
            protected override void OnRenderItemCheck (ToolStripItemImageRenderEventArgs e) {
                base.OnRenderItemCheck (e);
            }
            
            protected override void OnRenderItemText (ToolStripItemTextRenderEventArgs e) {
                //e.Graphics.Clear (Properties.Settings.Default.MenuStripInActiveDropDownBackColor);
            }

            protected override void OnRenderLabelBackground (ToolStripItemRenderEventArgs e) {
                base.OnRenderLabelBackground (e);
            }
            protected override void OnRenderOverflowButtonBackground (ToolStripItemRenderEventArgs e) {
                base.OnRenderOverflowButtonBackground (e);
            }
            protected override void OnRenderSeparator (ToolStripSeparatorRenderEventArgs e) {
                base.OnRenderSeparator (e);
            }
            protected override void OnRenderSplitButtonBackground (ToolStripItemRenderEventArgs e) {
                base.OnRenderSplitButtonBackground (e);
            }
            protected override void OnRenderStatusStripSizingGrip (ToolStripRenderEventArgs e) {
                base.OnRenderStatusStripSizingGrip (e);
            }
            protected override void OnRenderToolStripBackground (ToolStripRenderEventArgs e) {
                e.Graphics.Clear (Properties.Settings.Default.MenuStripInActiveDropDownBackColor);
            }

            protected override void OnRenderToolStripContentPanelBackground (ToolStripContentPanelRenderEventArgs e) {
                base.OnRenderToolStripContentPanelBackground (e);
            }
            protected override void OnRenderToolStripPanelBackground (ToolStripPanelRenderEventArgs e) {
                base.OnRenderToolStripPanelBackground (e);
            }
            protected override void OnRenderToolStripStatusLabelBackground (ToolStripItemRenderEventArgs e) {
                base.OnRenderToolStripStatusLabelBackground (e);
            }
        }
    }
}
