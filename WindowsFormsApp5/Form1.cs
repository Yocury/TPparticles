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
        
        // Настройки сложности
        private float minSpawnDelay;
        private float maxSpawnDelay;
        private int particleRadius;
        private float redParticleProbability;
        private float particleLifeTime; // Добавляем время жизни как параметр сложности

        // Перечисление для уровней сложности
        private enum Difficulty { Easy, Medium, Hard }

        public Form1()
        {
            InitializeComponent();

            // Показываем меню выбора сложности
            using (var menuForm = new MenuForm())
            {
                if (menuForm.ShowDialog() != DialogResult.OK)
                {
                    // Если пользователь закрыл окно без выбора, закрываем игру
                    Application.Exit();
                    return;
                }

                // Устанавливаем параметры в зависимости от выбранной сложности
                SetDifficultyParameters((MenuForm.Difficulty)menuForm.SelectedDifficulty);
            }

            // Создаем bitmap после инициализации формы
            if (picDisplay != null)
            {
                picDisplay.Image = new Bitmap(picDisplay.Width, picDisplay.Height);
                timer1.Interval = 16;
                timer1.Start();
            }
        }

        private void SetDifficultyParameters(MenuForm.Difficulty difficulty)
        {
            switch (difficulty)
            {
                case MenuForm.Difficulty.Easy:
                    minSpawnDelay = 0.3f;
                    maxSpawnDelay = 0.6f;
                    particleRadius = 20;
                    redParticleProbability = 0.4f;
                    particleLifeTime = 60f;
                    break;
                case MenuForm.Difficulty.Medium:
                    minSpawnDelay = 0.15f;
                    maxSpawnDelay = 0.4f;
                    particleRadius = 15;
                    redParticleProbability = 0.6f;
                    particleLifeTime = 35f; // Уменьшили с 45f до 35f для более быстрого исчезновения
                    break;
                case MenuForm.Difficulty.Hard:
                    minSpawnDelay = 0.05f;
                    maxSpawnDelay = 0.2f;
                    particleRadius = 10;
                    redParticleProbability = 0.8f;
                    particleLifeTime = 40f; // Увеличили с 30f до 40f для небольшого увеличения времени жизни
                    break;
            }
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
                    // Проверяем вероятность появления красной частицы
                    if (random.NextDouble() < redParticleProbability)
                    {
                        var particle = new Particle
                        {
                            X = random.Next(50, picDisplay.Width - 50),
                            Y = random.Next(50, picDisplay.Height - 50),
                            Radius = particleRadius,
                            Color = Color.Red,
                            Opacity = 0,
                            LifeTime = particleLifeTime,
                            IsAppearing = true
                        };
                        particles.Add(particle);
                    }
                    redSpawnDelay = (float)(random.NextDouble() * (maxSpawnDelay - minSpawnDelay) + minSpawnDelay);
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
                        Radius = particleRadius,
                        Color = Color.Green,
                        Opacity = 0,
                        LifeTime = particleLifeTime,
                        IsAppearing = true
                    };
                    particles.Add(particle);
                    greenCount++;
                    greenSpawnDelay = (float)(random.NextDouble() * (maxSpawnDelay - minSpawnDelay) + minSpawnDelay);
                }
                else
                {
                    greenSpawnDelay -= deltaTime;
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (picDisplay.Image == null) return;

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
                        greenSpawnDelay = (float)(random.NextDouble() * (maxSpawnDelay - minSpawnDelay) + minSpawnDelay);
                    }
                    break;
                }
            }
        }
    }
}
