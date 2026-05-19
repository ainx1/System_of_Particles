using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace particles
{
    public abstract class IImpactPoint
    {
        public float X;
        public float Y;

        // абстрактный метод с помощью которого будем изменять состояние частиц
        // например притягивать
        public abstract void ImpactParticle(Particle particle);

        public virtual void Render(Graphics g) // добавил слово virtual, ну чтобы override потом можно было юзать
        {
            /* ... */
        }
    }

    // задача 5
    public class ColorPoint : IImpactPoint // задача 5
    {
        public Color Color = Color.White; // цветв который красим
        public int Power = 60;

        public override void ImpactParticle(Particle particle)
        {
            // считаем расстояние как в гравитони
            float gX = X - particle.X;
            float gY = Y - particle.Y;

            double r = Math.Sqrt(gX * gX + gY * gY);

            // если частица внутри круга
            if (r < Power / 2)
            {
                // базовую частицу в цветную
                var colorful = particle as Particle.ParticleColorful;
                
                    colorful.FromColor = Color; // замена цвета
            }
        }
        public override void Render(Graphics g)
        {

            g.DrawEllipse(
                new Pen(Color, 2),
                X - Power / 2,
                Y - Power / 2,
                Power,
                Power
            );
        }
    }
    // задача 6 счетчик частиц, которые попали в зону
    public class CounterPoint : IImpactPoint
    {
        public int Count = 0; // счетчик
        public int Power = 80;

        public override void ImpactParticle(Particle particle)
        {
            float gX = X - particle.X;
            float gY = Y - particle.Y;
            double r = Math.Sqrt(gX * gX + gY * gY);

            // если частица попала в радиус
            if (r < Power / 2)
            {
                // если частица еще "жива"
                if (particle.Life > 0)
                {
                    particle.Life = 0;
                    Count++; // увеличиваем счетчик
                }
            }
        }
        
        public override void Render(Graphics g)
        {
            // рисуем саму зону
            g.DrawEllipse(new Pen(Color.Yellow, 2), X - Power / 2, Y - Power / 2, Power, Power);

            var stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;

            var text = $"{Count}"; 
            var font = new Font("Verdana", 12, FontStyle.Bold);

            // число в центре точки
            g.DrawString(
                text,
                font,
                new SolidBrush(Color.White),
                X,
                Y,
                stringFormat
            );
        }
    }

    // задача 8
    public class RadarPoint : IImpactPoint
    {
        public int Power = 100;
                                
        private List<PointF> detectedParticles = new List<PointF>();
        public override void ImpactParticle(Particle particle)
        {
            float gX = X - particle.X;
            float gY = Y - particle.Y;
            double r = Math.Sqrt(gX * gX + gY * gY);

            // если частица попала в зону радара
            if (r < Power / 2)
            {
                // запоминаем её координаты
                detectedParticles.Add(new PointF(particle.X, particle.Y));
            }
        }

        public override void Render(Graphics g)
        {
            // окружность радара
            g.DrawEllipse(new Pen(Color.Lime, 2), X - Power / 2, Y - Power / 2, Power, Power);

            // копии для каждой засеченной частицы
            foreach (var pos in detectedParticles)
            {

                g.DrawEllipse(new Pen(Color.Lime, 2), pos.X - 5, pos.Y - 5, 10, 10);
            }

            // текст с количеством в центре
            var stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;

            var text = $"{detectedParticles.Count}";
            var font = new Font("Verdana", 12, FontStyle.Bold);

            g.DrawString(text, font, new SolidBrush(Color.Lime), X, Y, stringFormat);

            // очищаем список после отрисовки чтобы не было частиц с позапрошлых кадров и тд
            detectedParticles.Clear();
        }
    }

    public class GravityPoint : IImpactPoint
        {
            public int Power = 100; // сила притяжения

            // а сюда по сути скопировали с минимальными правками то что было в UpdateState
            public override void ImpactParticle(Particle particle)
            {
                // и так считаем вектор притяжения к точке
                float gX = X - particle.X;
                float gY = Y - particle.Y;
                // считаем квадрат расстояния между частицей и точкой r^2
                double r = Math.Sqrt(gX * gX + gY * gY); // считаем расстояние от центра точки до центра частицы
                if (r + particle.Radius < Power / 2) // если частица оказалось внутри окружности
                {
                // то притягиваем ее
                float r2 = (float)Math.Max(100, gX * gX + gY * gY);
                particle.SpeedX += gX * Power / r2;
                particle.SpeedY += gY * Power / r2;
                }
            }

            // базовый класс для отрисовки точечки
            public override void Render(Graphics g)
            {
                // буду рисовать окружность с диаметром равным Power
                g.DrawEllipse(
                new Pen(Color.Red),
                X - Power / 2,
                Y - Power / 2,
                Power,
                Power
                );

                g.DrawString(
                $"Я гравитон\nc силой {Power}", // надпись, можно перенос строки вставлять
                new Font("Verdana", 10), // шрифт и его размер
                new SolidBrush(Color.White), // цвет шрифта
                X, // расположение в пространстве
                Y
                );
            }
    }
    

    public class AntiGravityPoint : IImpactPoint
    {
        public int Power = 100; // сила отторжения

        // а сюда по сути скопировали с минимальными правками то что было в UpdateState
        public override void ImpactParticle(Particle particle)
        {
            float gX = X - particle.X;
            float gY = Y - particle.Y;
            float r2 = (float)Math.Max(100, gX * gX + gY * gY);

            particle.SpeedX -= gX * Power / r2; // тут минусики вместо плюсов
            particle.SpeedY -= gY * Power / r2; // и тут
        }
        // базовый класс для отрисовки точечки
        public override void Render(Graphics g)
        {
            // буду рисовать окружность с диаметром равным Power
            g.DrawEllipse(
                   new Pen(Color.Red),
                   X - Power / 2,
                   Y - Power / 2,
                   Power,
                   Power
               );
        }
    }

}