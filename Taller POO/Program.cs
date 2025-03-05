using System;
using System.Collections.Generic;
using System.Threading;  

namespace Taller_POO
{
    public abstract class Node
    {
        protected List<Node> children = new List<Node>();

        public void AddChild(Node child)
        {
            children.Add(child);
        }

        public abstract bool Execute();
    }

    public abstract class Composite : Node { }

    public class Selector : Composite
    {
        public override bool Execute()
        {
            foreach (var child in children)
            {
                if (child.Execute()) return true;  
            }
            return false; 
        }
    }

    public class Sequence : Composite
    {
        public override bool Execute()
        {
            foreach (var child in children)
            {
                if (!child.Execute()) return false;  
            }
            return true;  
        }
    }

    public class CheckDistanceTask : Node
    {
        private float objectDistance;
        private float validDistance;

        public float ObjectDistance => objectDistance;
        public float ValidDistance => validDistance;

        public CheckDistanceTask(float objDist, float validDist)
        {
            objectDistance = objDist;
            validDistance = validDist;
        }

        public override bool Execute()
        {
            if (objectDistance <= validDistance)
            {
                Console.WriteLine("Objeto dentro de la distancia válida.");
                return true;
            }
            Console.WriteLine("Objeto fuera de la distancia válida.");
            return false;
        }
    }

    public class MoveToTargetTask : Node
    {
        private CheckDistanceTask checkDistanceTask;
        private float position;
        private float stepSize;

        public MoveToTargetTask(CheckDistanceTask checkDistance, float step = 1.0f)
        {
            checkDistanceTask = checkDistance;
            position = checkDistanceTask.ObjectDistance;
            stepSize = step;
        }

        public override bool Execute()
        {
            float targetPosition = checkDistanceTask.ValidDistance;

            Console.WriteLine("Iniciando movimiento hacia el objetivo...");

            while (position < targetPosition)
            {
                position += stepSize;
                if (position > targetPosition) position = targetPosition;

                Console.WriteLine($"Posición actual: {position}");
                Thread.Sleep(500); 
            }

            Console.WriteLine("Objetivo alcanzado.");
            return true;
        }
    }

    public class WaitTask : Node
    {
        private int waitTime;

        public WaitTask(int time)
        {
            waitTime = time;
        }

        public override bool Execute()
        {
            Console.WriteLine($" Esperando {waitTime / 1000} segundos...");
            Thread.Sleep(waitTime);
            return true;
        }
    }

    class Program
    {
        static void Main()
        {
            Sequence root = new Sequence();

            Selector selector = new Selector();
            Sequence moveSequence = new Sequence();
            Selector nonEvaluatingSelector = new Selector(); 

            int tiempoEspera = 2000;  

            CheckDistanceTask checkDistance = new CheckDistanceTask(3.0f, 16.0f);
            MoveToTargetTask moveToTarget = new MoveToTargetTask(checkDistance, 0.5f);
            WaitTask wait = new WaitTask(tiempoEspera);

            moveSequence.AddChild(checkDistance);
            moveSequence.AddChild(moveToTarget);

            selector.AddChild(moveSequence);
            selector.AddChild(wait);  

            nonEvaluatingSelector.AddChild(selector);

            Sequence mainSequence = new Sequence();
            mainSequence.AddChild(nonEvaluatingSelector);
            mainSequence.AddChild(wait);

            
            root.AddChild(mainSequence);

            Console.WriteLine("Ejecutando Árbol de Comportamiento en bucle infinito:");
            while (true)
            {
                root.Execute();
            }
        }
    }
}
