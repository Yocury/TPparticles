using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static WindowsFormsApp5.IImpactPoint;

namespace WindowsFormsApp5
{
    public partial class Form1 : Form
    {
        private List<Particle> particles = new List<Particle>();
        private Random random = new Random();
        private const int MAX_PARTICLES = 3;

        public Form1()
        {
            InitializeComponent();
            picDisplay.Image = new Bitmap(picDisplay.Width, picDisplay.Height);

            // Set up timer
            timer1.Interval = 50;
            timer1.Start();

            // Initial particle creation
            CreateParticlesIfNeeded();
        }

        private void CreateParticlesIfNeeded()
        {
            while (particles.Count < MAX_PARTICLES)
            {
                var particle = new Particle
                {
                    X = random.Next(50, picDisplay.Width - 50),
                    Y = random.Next(50, picDisplay.Height - 50),
                    Radius = 20
                };
                particles.Add(particle);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            CreateParticlesIfNeeded();

            using (var g = Graphics.FromImage(picDisplay.Image))
            {
                g.Clear(Color.White);
                foreach (var particle in particles)
                {
                    using (var brush = new SolidBrush(Color.Green))
                    {
                        g.FillEllipse(brush, particle.X - particle.Radius, 
                            particle.Y - particle.Radius, 
                            particle.Radius * 2, 
                            particle.Radius * 2);
                    }
                }
            }
            picDisplay.Invalidate();
        }

        private void picDisplay_MouseClick(object sender, MouseEventArgs e)
        {
            for (int i = particles.Count - 1; i >= 0; i--)
            {
                var particle = particles[i];
                // Check if click is within particle bounds
                if (Math.Pow(e.X - particle.X, 2) + Math.Pow(e.Y - particle.Y, 2) <= Math.Pow(particle.Radius, 2))
                {
                    particles.RemoveAt(i);
                    break;
                }
            }
        }
    }
}
