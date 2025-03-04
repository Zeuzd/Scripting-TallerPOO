using System;
using System.Collections.Generic;

namespace Taller_POO
{
    using System;
    using System.Collections.Generic;

    // Clase base abstracta Node
    public abstract class Node
    {
        protected List<Node> children = new List<Node>();

        public void AddChild(Node child)
        {
             children.Add(child);
        }
    }

    public abstract bool Execute();
    }

    // Clase Composite (para nodos que contienen hijos)
    public abstract class Composite : Node
    {
        protected List<Node> children = new List<Node>();

        public void AddChild(Node child)
        {
            children.Add(child);
        }
    }

    // Clase Selector (intenta ejecutar hijos hasta que uno tenga éxito)
    public class Selector : Composite
    {
        public override bool Execute()
        {
            foreach (var child in children)
            {
                if (child.Execute())
                {
                    return true;  // Un hijo tuvo éxito, se detiene el Selector
                }
            }
            return false;  // Todos fallaron
        }
    }

    // Clase Sequence (ejecuta todos los hijos en orden hasta que uno falle)
    public class Sequence : Composite
    {
        public override bool Execute()
        {
            foreach (var child in children)
            {
                if (!child.Execute())
                {
                    return false;  // Un hijo falló, la secuencia no se completa
                }
            }
            return true;  // Todos tuvieron éxito
        }
    }

    // Tareas concretas (nodos hoja)
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
                Console.WriteLine(" Objeto dentro de la distancia válida");
                return true;
            }
            Console.WriteLine(" Objeto fuera de la distancia válida");
            return false;
        }
    }

    public class MoveTask : Node
    {
        public override bool Execute()
        {
            Console.WriteLine(" Moviendo al objetivo...");
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

            CheckDistanceTask checkDistance = new CheckDistanceTask(7.0f, 5.0f);
            MoveTask moveToTarget = new MoveTask();
            WaitTask wait = new WaitTask();

            // Configurar el árbol de comportamiento
            moveSequence.AddChild(checkDistance);
            moveSequence.AddChild(moveToTarget);  // Se mueve si está cerca

            selector.AddChild(moveSequence);
            selector.AddChild(wait);  // Si la distancia no es válida, espera

            root.AddChild(selector);

            // Ejecutar comportamiento
            Console.WriteLine(" Ejecutando Árbol de Comportamiento:");
            root.Execute();
        }
    }

}
