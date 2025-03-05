using System;
using System.Collections.Generic;
using System.Threading;  

namespace Taller_POO
{
    // Clase abstracta para los nodos del árbol
    public abstract class Node
    {
        // Esta es la lista de hijos que pueden hacer las tareas o tener mas nodos
        protected List<Node> children = new List<Node>();
        //Método para agregar hijos al nodo
        public void AddChild(Node child)
        {
            children.Add(child);
        }

        // Método que ejecuta la lógica del nodo
        public abstract bool Execute();
    }

    // Clase con la que unimos los nodos
    public abstract class Composite : Node { }

    //Selector: Con esto evaluamos los hijos y si al menos uno tiene exito retorna true
    public class Selector : Composite
    {
        public override bool Execute()
        {
            foreach (var child in children)
            {
                if (child.Execute()) return true;  
            }
            return false; // Retorna false si NINGUNOOO tiene exito 
        }
    }

    // Secuencia: Esta al contrario que selector necesita que TODOS tengan exito 
    public class Sequence : Composite
    {
        public override bool Execute()
        {
            foreach (var child in children) // Evalua los hijos en orden
            {
                if (!child.Execute()) return false;  // Si uno de ellos falla, la secuencia falla
            }
            return true;  // Solo si todos toditos los hijos tienen exito retorna true
        }
    }

    // PUNTO A: Evaluar la distancia del objeto 
    // Con este verificaremos si la distancia del objeto es menor o igual a la distancia válida
    public class CheckDistanceTask : Node
    {
        private float objectDistance; // distancia del objeto
        private float validDistance; // distancia válida

        public float ObjectDistance => objectDistance;// Aqui permitimos que se accedan a los datos sin modificarlos desde fuera de la clase
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
                return true; // retorna true si esta dentro del rango
            }
            Console.WriteLine("Objeto fuera de la distancia válida.");
            return false; // y false si esta fuera del rango
        }
    }

    // Punto B: Movemos el objetivo paso a paso hasta el objetivo SI la distancia es válida
    public class MoveToTargetTask : Node
    {
        private CheckDistanceTask checkDistanceTask; // referencia de la verificacion de distancia
        private float position; // posicion actual del objeto
        private float stepSize; // tamaño del paso

        public MoveToTargetTask(CheckDistanceTask checkDistance, float step = 1.0f)
        {
            checkDistanceTask = checkDistance;
            position = checkDistanceTask.ObjectDistance; // empezamos desde la distancia inicial
            stepSize = step;
        }

        public override bool Execute()
        {
            float targetPosition = checkDistanceTask.ValidDistance; // esta es la posicion destino

            Console.WriteLine("Iniciando movimiento hacia el objetivo...");

            while (position < targetPosition)
            {
                position += stepSize;
                if (position > targetPosition) position = targetPosition; // con esto aseguramos no sobre pasar el objetivo 

                Console.WriteLine($"Posición actual: {position}");
                Thread.Sleep(500); // con esto simulamos el movimiento, para que no se ejecute todo de corrido
            }

            Console.WriteLine("Objetivo alcanzado.");
            return true; // cuando acabamos el movimiento retorna true 
        }
    }

    // Punto C: Esperar antes de volver a evaluar en la secuencia 
    public class WaitTask : Node
    {
        private int waitTime; // este es el tiempo de espera

        public WaitTask(int time)
        {
            waitTime = time;
        }

        public override bool Execute()
        {
            Console.WriteLine($" Esperando {waitTime / 1000} segundos...");
            Thread.Sleep(waitTime); // pausamo el programa por el tiempo especificado
            return true;
        }
    }

    class Program
    {
        static void Main()
        {
            // esto es del C, creamos la secuencia raíz
            Sequence root = new Sequence();
            
            // del punto A, este es el selector que evalua la distancia
            Selector selector = new Selector();

            // Punto B: secuencia que mueve el objetivo si la distancia es válida
            Sequence moveSequence = new Sequence();

            // punto C, es un selecotr que no evalúa condiciones
            Selector nonEvaluatingSelector = new Selector(); 

            int tiempoEspera = 2000; // esto es lo que esperamos antes de repetir el código 

            // Punto A
            CheckDistanceTask checkDistance = new CheckDistanceTask(3.0f, 16.0f);
            //Punto B
            MoveToTargetTask moveToTarget = new MoveToTargetTask(checkDistance, 0.5f);
            // Punto C
            WaitTask wait = new WaitTask(tiempoEspera);

            //Punto B
            moveSequence.AddChild(checkDistance);
            moveSequence.AddChild(moveToTarget);

            //Punto A
            selector.AddChild(moveSequence);
            selector.AddChild(wait);  

            //Punto C
            nonEvaluatingSelector.AddChild(selector);

            Sequence mainSequence = new Sequence();
            mainSequence.AddChild(nonEvaluatingSelector);
            mainSequence.AddChild(wait);

            
            root.AddChild(mainSequence);

            Console.WriteLine("Ejecutando Árbol de Comportamiento en bucle infinito:");
            while (true)
            {
                root.Execute(); // Esto lo repite indefinidamente 
            }
        }
    }
}
