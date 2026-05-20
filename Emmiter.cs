using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static particles.Particle;

namespace particles
{

    public class Emitter
    {
        // собственно список
        List<Particle> particles = new List<Particle>();
        public float GravitationX = 0;
        public float GravitationY = 0;

        public List<IImpactPoint> impactPoints = new List<IImpactPoint>(); // <<< ТАК ВОТ

        /* добавил метод */
        public virtual Particle CreateParticle()
        {
            var particle = new ParticleColorful();
            particle.FromColor = ColorFrom;
            particle.ToColor = ColorTo;

            return particle;
        }

        public void UpdateState()
        {
            int particlesToCreate = ParticlesPerTick; // фиксируем счетчик сколько частиц нам создавать за тик

            foreach (var particle in particles)
            {
                // если здоровье кончилось
                if (particle.Life <= 0) // если частицы умерла
                {
                     //проверяем надо ли создать частицу
                    if (particlesToCreate > 0)
                    {
                        /* у нас как сброс частицы равносилен созданию частицы */
                        particlesToCreate -= 1; // поэтому уменьшаем счётчик созданных частиц на 1
                        ResetParticle(particle);
                    }
                }
                else
                {
                    /* теперь двигаю вначале */
                    particle.X += particle.SpeedX;
                    particle.Y += particle.SpeedY;
                    // каждая точка по-своему воздействует на вектор скорости

                    particle.Life -= 1; // уменьшаю здоровье

                    foreach (var point in impactPoints)
                    {
                        point.ImpactParticle(particle);
                    }

                    // гравитация воздействует на вектор скорости, поэтому пересчитываем его
                    particle.SpeedX += GravitationX;
                    particle.SpeedY += GravitationY;

                }
            }

            // добавил генерацию частиц
            // генерирую не более 10 штук за тик
            while (particlesToCreate >= 1)
            {
                if (particles.Count < ParticlesCount) // пока частиц меньше 500 генерируем новые
                {
                    particlesToCreate -= 1;
                    var particle = CreateParticle();
                    ResetParticle(particle);
                    particles.Add(particle);
                }
                else
                {
                    break; // если частиц 500 штук, то ничего не генерирую
                }
            }

        }
        public void Render(Graphics g)
        {

            // это то же самое что на форме в методе Render
            foreach (var particle in particles)
            {
                particle.Draw(g);
            }

            // рисую точки притяжения красными кружочками
            foreach (var point in impactPoints)
            {
                point.Render(g); 
            }
        }

        public int X; // координата X центра эмиттера, будем ее использовать вместо MousePositionX
        public int Y; // соответствующая координата Y 
        public int Direction = 0; // вектор направления в градусах куда сыпет эмиттер
        public int Spreading = 360; // разброс частиц относительно Direction
        public int SpeedMin = 1; // начальная минимальная скорость движения частицы
        public int SpeedMax = 10; // начальная максимальная скорость движения частицы
        public int RadiusMin = 2; // минимальный радиус частицы
        public int RadiusMax = 10; // максимальный радиус частицы
        public int LifeMin = 20; // минимальное время жизни частицы
        public int LifeMax = 100; // максимальное время жизни частицы
        public int ParticlesPerTick = 1; // добавил новое поле
        public int ParticlesCount = 500;

        public Color ColorFrom = Color.White; // начальный цвет частицы
        public Color ColorTo = Color.FromArgb(0, Color.Black); // конечный цвет частиц

        // добавил новый метод, виртуальным, чтобы переопределять можно было
        public virtual void ResetParticle(Particle particle)
        {
            // восстанавливаю здоровье
            particle.Life = Particle.rand.Next(LifeMin, LifeMax);
            // новое начальное расположение частицы — это то, куда указывает курсор
            particle.X = X;
            particle.Y = Y;

            /* ЭТО ДОБАВЛЯЮ, тут сброс состояния частицы */
            var direction = Direction
                + (double)Particle.rand.Next(Spreading)
                - Spreading / 2;

            var speed = Particle.rand.Next(SpeedMin, SpeedMax);

            particle.SpeedX = (float)(Math.Cos(direction / 180 * Math.PI) * speed);
            particle.SpeedY = -(float)(Math.Sin(direction / 180 * Math.PI) * speed);
            /* конец ЭТО ДОБАВЛЯЮ  */
            particle.Radius = Particle.rand.Next(RadiusMin, RadiusMax);

            // Задача 5 ДЛЯ СБРОСА ЦВЕТА надо было сделать 
            if (particle is ParticleColorful colorful)
            {
                colorful.FromColor = ColorFrom;
                colorful.ToColor = ColorTo;
            }

        }

        public class TopEmitter : Emitter
        {
            public int Width; // длина экрана

            public override void ResetParticle(Particle particle)
            {
                base.ResetParticle(particle); // вызываем базовый сброс частицы, там жизнь переопределяется и все такое

                // а теперь тут уже подкручиваем параметры движения
                particle.X = Particle.rand.Next(Width); // позиция X -- произвольная точка от 0 до Width
                particle.Y = 0;  // ноль -- это верх экрана 

                particle.SpeedY = 1; // падаем вниз по умолчанию
                particle.SpeedX = Particle.rand.Next(-2, 2); // разброс влево и вправа у частиц 
            }
        }
    }
}
