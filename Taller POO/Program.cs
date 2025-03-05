using System;
using System.Collections.Generic;

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

        public CheckDistanceTask(float objDist, float validDist)
        {
            objectDistance = objDist;
            validDistance = validDist;
        }

        public override bool Execute()
        {
            if (objectDistance <= validDistance)
            {
                Console.WriteLine(" Objeto dentro de la distancia válida.");
                return true;
            }
            Console.WriteLine(" Objeto fuera de la distancia válida.");
            return false;
        }
    }

    public class MoveToTargetTask : Node
    {
        private float position;
        private float targetPosition;
        private float stepSize;

        public MoveToTargetTask(float startPosition, float target, float step = 1.0f)
        {
            position = startPosition;
            targetPosition = target;
            stepSize = step;
        }

        public override bool Execute()
        {
            Console.WriteLine(" Iniciando movimiento hacia el objetivo...");

            while (position < targetPosition)
            {
                position += stepSize;
                if (position > targetPosition) position = targetPosition;

                Console.WriteLine($" Posición actual: {position}");
                System.Threading.Thread.Sleep(500); 
            }

            Console.WriteLine(" Objetivo alcanzado.");
            return true;
        }
    }

    public class WaitTask : Node
    {
        public override bool Execute()
        {
            Console.WriteLine(" Esperando...");
            return true;
        }
    }

    class Program
    {
        static void Main()
        {
            // Crear nodos
            Sequence root = new Sequence();

            Selector selector = new Selector();
            Sequence moveSequence = new Sequence();

            CheckDistanceTask checkDistance = new CheckDistanceTask(1.0f, 4.0f);
            MoveToTargetTask moveToTarget = new MoveToTargetTask(1.0f, 4.0f, 0.5f);
            WaitTask wait = new WaitTask();

            // Configurar el árbol de comportamiento
            moveSequence.AddChild(checkDistance);
            moveSequence.AddChild(moveToTarget);

            selector.AddChild(moveSequence);
            selector.AddChild(wait);  // Si la distancia no es válida, espera

            root.AddChild(selector);

            // Ejecutar comportamiento
            Console.WriteLine(" Ejecutando Árbol de Comportamiento:");
            root.Execute();
        }
    }
}

