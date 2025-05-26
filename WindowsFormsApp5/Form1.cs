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
        private const int MAX_GREEN_PARTICLES = 3;
        
        // Таймеры для задержки создания частиц
        private float redSpawnDelay = 0;
        private float greenSpawnDelay = 0;
        
        // Константы для настройки задержек (уменьшены в 2-3 раза)
        private const float MIN_SPAWN_DELAY = 0.2f; // Минимальная задержка в секундах (было 0.5)
        private const float MAX_SPAWN_DELAY = 0.7f; // Максимальная задержка в секундах (было 2.0)

        public Form1()
        {
            InitializeComponent();
            picDisplay.Image = new Bitmap(picDisplay.Width, picDisplay.Height);

            // Set up timer
            timer1.Interval = 16; // Примерно 60 FPS
            timer1.Start();

            // Initial particle creation
            CreateParticlesIfNeeded();
        }

        private void CreateParticlesIfNeeded()
        {
            // Удаляем мертвые частицы
            particles.RemoveAll(p => p.IsDead());

            // Подсчитываем текущее количество частиц каждого цвета
            int greenCount = particles.Count(p => p.Color == Color.Green);
            int redCount = particles.Count(p => p.Color == Color.Red);

            // Обработка таймеров
            float deltaTime = timer1.Interval / 1000f; // Переводим миллисекунды в секунды

            // Обработка появления красной частицы
            if (redCount == 0)
            {
                if (redSpawnDelay <= 0)
                {
                    var particle = new Particle
                    {
                        X = random.Next(50, picDisplay.Width - 50),
                        Y = random.Next(50, picDisplay.Height - 50),
                        Radius = 20,
                        Color = Color.Red,
                        Opacity = 0,
                        LifeTime = 100,
                        IsAppearing = true
                    };
                    particles.Add(particle);
                    // Устанавливаем новую случайную задержку
                    redSpawnDelay = (float)(random.NextDouble() * (MAX_SPAWN_DELAY - MIN_SPAWN_DELAY) + MIN_SPAWN_DELAY);
                }
                else
                {
                    redSpawnDelay -= deltaTime;
                }
            }

            // Обработка появления зеленых частиц
            if (greenCount < MAX_GREEN_PARTICLES)
            {
                if (greenSpawnDelay <= 0)
                {
                    var particle = new Particle
                    {
                        X = random.Next(50, picDisplay.Width - 50),
                        Y = random.Next(50, picDisplay.Height - 50),
                        Radius = 20,
                        Color = Color.Green,
                        Opacity = 0,
                        LifeTime = 100,
                        IsAppearing = true
                    };
                    particles.Add(particle);
                    greenCount++;
                    // Устанавливаем новую случайную задержку
                    greenSpawnDelay = (float)(random.NextDouble() * (MAX_SPAWN_DELAY - MIN_SPAWN_DELAY) + MIN_SPAWN_DELAY);
                }
                else
                {
                    greenSpawnDelay -= deltaTime;
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            // Обновляем состояние частиц
            foreach (var particle in particles)
            {
                particle.Update();
            }

            CreateParticlesIfNeeded();

            using (var g = Graphics.FromImage(picDisplay.Image))
            {
                g.Clear(Color.White);
                foreach (var particle in particles)
                {
                    using (var brush = new SolidBrush(Color.FromArgb(
                        (int)(particle.Opacity * 255),
                        particle.Color)))
                    {
                        g.FillEllipse(brush,
                            particle.X - particle.Radius,
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
                // Проверяем попадание клика в частицу
                if (Math.Pow(e.X - particle.X, 2) + Math.Pow(e.Y - particle.Y, 2) <= Math.Pow(particle.Radius, 2))
                {
                    // Убираем только зеленые частицы при клике
                    if (particle.Color == Color.Green)
                    {
                        particles.RemoveAt(i);
                        // Устанавливаем задержку для появления новой зеленой частицы
                        greenSpawnDelay = (float)(random.NextDouble() * (MAX_SPAWN_DELAY - MIN_SPAWN_DELAY) + MIN_SPAWN_DELAY);
                    }
                    break;
                }
            }
        }
    }
}
