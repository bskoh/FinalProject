using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinFormAnimation;

namespace Test
{
    public partial class Form1 : Form
    {
        private readonly Animator2D pick_to_pre_drop = new Animator2D();
        private readonly Animator2D predrop_to_dropstay = new Animator2D();
        private readonly Animator2D dropstay_to_drop = new Animator2D();
        private readonly Animator2D drop_to_prepick = new Animator2D();
        private readonly Animator2D prepick_to_pickstay = new Animator2D();
        private readonly Animator2D pickstay_to_pick = new Animator2D();

        int count = 0;

        public Form1()
        {
            InitializeComponent();

            pb_Line.Location = new Point(90, 75);
            pb_AGV.Location = new Point(80, 275);
            pb_Line.SizeMode = PictureBoxSizeMode.StretchImage;
            pb_AGV.SizeMode = PictureBoxSizeMode.StretchImage;


            pick_to_pre_drop.Paths = new Path2D(new Path(80, 80, 1000, AnimationFunctions.Liner),
                new Path(275, 385, 1000, AnimationFunctions.Liner))
                .ContinueTo(new Path2D(
                    new Path(80, 190, 1000, AnimationFunctions.Liner),
                    new Path(385, 385, 1000, AnimationFunctions.Liner)));

            predrop_to_dropstay.Paths = new Path2D(new Path(190, 240, 1000, AnimationFunctions.Liner),
                new Path(385, 385, 1000, AnimationFunctions.Liner))
                .ContinueTo(new Path2D(
                    new Path(240, 440, 4000, AnimationFunctions.Liner),
                    new Path(385, 575, 4000, AnimationFunctions.Liner))
                .ContinueTo(new Path2D(
                    new Path(440, 490, 1000, AnimationFunctions.Liner),
                    new Path(575, 575, 1000, AnimationFunctions.Liner))));

            dropstay_to_drop.Paths = new Path2D(new Path(490, 610, 1000, AnimationFunctions.Liner),
                new Path(575, 575, 1000, AnimationFunctions.Liner))
                .ContinueTo(new Path2D(
                    new Path(610, 610, 2000, AnimationFunctions.Liner),
                    new Path(575, 375, 2000, AnimationFunctions.Liner)));

            drop_to_prepick.Paths = new Path2D(new Path(610, 610, 1000, AnimationFunctions.Liner),
                new Path(375, 260, 1000, AnimationFunctions.Liner))
                .ContinueTo(new Path2D(
                    new Path(610, 520, 1000, AnimationFunctions.Liner),
                    new Path(260, 260, 1000, AnimationFunctions.Liner)));

            prepick_to_pickstay.Paths = new Path2D(new Path(520, 450, 1000, AnimationFunctions.Liner),
                new Path(260, 260, 1000, AnimationFunctions.Liner))
                .ContinueTo(new Path2D(
                    new Path(450, 250, 2000, AnimationFunctions.Liner),
                    new Path(260, 75, 2000, AnimationFunctions.Liner))
                .ContinueTo(new Path2D(
                    new Path(250, 200, 1000, AnimationFunctions.Liner),
                    new Path(75, 75, 1000, AnimationFunctions.Liner))));

            pickstay_to_pick.Paths = new Path2D(new Path(200, 80, 1000, AnimationFunctions.Liner),
                new Path(75, 75, 1000, AnimationFunctions.CubicEaseOut))
                .ContinueTo(new Path2D(
                    new Path(80, 80, 2000, AnimationFunctions.Liner),
                    new Path(75, 275, 2000, AnimationFunctions.Liner)));
        }

        private void btn_rfid_Click(object sender, EventArgs e)
        {
            if(count == 0)
            {
                pick_to_pre_drop.Play(pb_AGV, Animator2D.KnownProperties.Location);
            }
            else if(count == 1)
            {
                predrop_to_dropstay.Play(pb_AGV, Animator2D.KnownProperties.Location);
            }
            else if(count == 2)
            {
                dropstay_to_drop.Play(pb_AGV, Animator2D.KnownProperties.Location);
            }
            else if(count == 3)
            {
                drop_to_prepick.Play(pb_AGV, Animator2D.KnownProperties.Location);
            }
            else if(count == 4)
            {
                prepick_to_pickstay.Play(pb_AGV, Animator2D.KnownProperties.Location);
            }
            else if(count == 5)
            {
                pickstay_to_pick.Play(pb_AGV, Animator2D.KnownProperties.Location);
            }
            count++;
            if(count > 5)
            {
                count = 0;
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            predrop_to_dropstay.Play(pb_AGV, Animator2D.KnownProperties.Location);
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            dropstay_to_drop.Play(pb_AGV, Animator2D.KnownProperties.Location);
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            drop_to_prepick.Play(pb_AGV, Animator2D.KnownProperties.Location);
        }

        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {
            prepick_to_pickstay.Play(pb_AGV, Animator2D.KnownProperties.Location);
        }

        private void radioButton6_CheckedChanged(object sender, EventArgs e)
        {
            pickstay_to_pick.Play(pb_AGV, Animator2D.KnownProperties.Location);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            pick_to_pre_drop.Play(pb_AGV, Animator2D.KnownProperties.Location);
        }
    }
}
