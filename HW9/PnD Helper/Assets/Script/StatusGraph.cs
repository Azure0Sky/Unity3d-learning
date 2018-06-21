using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusGraph
{
    private Hashtable graph;
    private StatusVertice begin;

    // The status when Help() is called
    public StatusVertice currVertice;

    private static readonly int[] priestMove = { 1, 0, 2, 0, 1 };
    private static readonly int[] devilMove = { 0, 1, 0, 2, 1 };
    private static readonly StatusVertice destination = new StatusVertice( 0, 0, false, 0, true );

    // The vertices of the graph
    public class StatusVertice
    {
        public int priest;                  // The number of priests on 'from' bank
        public int devil;                   // The number of devils on 'from' bank
        public bool boatAtFromBank;         // Whether the boat stops by the 'from' bank

        public int distance;                // The distance between this vertice to destination
        public bool arrived;                // For finding shortest path by Dijkstra's algorithm

        public StatusVertice nextVertice;               // The next vertice of this in the shortest path
        public List<StatusVertice> adjacentVertices;    // The adjacent vertices of this vertices
        
        public StatusVertice( int priestNum, int devilNum, bool fromBank, int _distance = int.MaxValue, bool _arrived = false )
        {
            priest = priestNum;
            devil = devilNum;
            boatAtFromBank = fromBank;

            distance = _distance;
            arrived = _arrived;

            adjacentVertices = new List<StatusVertice>();
        }

        public override int GetHashCode()
        {
            return GetHashCodeWithData( priest, devil, boatAtFromBank );
        }

        public static int GetHashCodeWithData( int priestNum, int devilNum, bool fromBank )
        {
            return 5 * priestNum - devilNum + ( fromBank ? 1 : -1 ) * 100;
        }

        public bool IsValid()
        {
            return priest == devil || priest == 3 || priest == 0;
        }
    }

    public StatusGraph( int initPriestNum, int initDevilNum, bool fromBank )
    {
        graph = new Hashtable();
        begin = new StatusVertice( initPriestNum, initDevilNum, fromBank );

        graph.Add( begin.GetHashCode(), begin );

        GenerateGraph();
        FindShortestPath();
    }

    // Use BFS to generate the status graph 
    private void GenerateGraph()
    {
        Queue<StatusVertice> verticeQueue = new Queue<StatusVertice>();
        verticeQueue.Enqueue( begin );

        while ( verticeQueue.Count > 0 ) {

            StatusVertice curr = verticeQueue.Peek();
            if ( !curr.IsValid() ) {
                verticeQueue.Dequeue();
                continue;
            }

            int factor = curr.boatAtFromBank ? -1 : 1;

            for ( int i = 0; i < priestMove.Length; ++i ) {         // priestMove.Length == devilMove.Length
                
                if ( ( curr.priest + priestMove[i]*factor >= 0 && curr.priest + priestMove[i]*factor <= 3 ) &&
                     ( curr.devil + devilMove[i]*factor >= 0 && curr.devil + devilMove[i]*factor <= 3 ) ) {

                    StatusVertice newVertice = 
                        new StatusVertice( curr.priest + priestMove[i] * factor, curr.devil + devilMove[i] * factor, !curr.boatAtFromBank );

                    curr.adjacentVertices.Add( newVertice );

                    if ( !graph.Contains( newVertice.GetHashCode() ) ) {
                        graph.Add( newVertice.GetHashCode(), newVertice );
                        verticeQueue.Enqueue( newVertice );
                    }

                }

            }

            verticeQueue.Dequeue();

        }
    }

    // Use Dijkstra's algorithm to find the shortest path
    // From the destination to the beginning
    private void FindShortestPath()
    {
        Queue<StatusVertice> verticeQueue = new Queue<StatusVertice>();
        verticeQueue.Enqueue( (StatusVertice)graph[destination.GetHashCode()] );

        while ( verticeQueue.Count > 0 ) {

            StatusVertice curr = verticeQueue.Peek();

            foreach ( StatusVertice temp in curr.adjacentVertices ) {

                var adjacent = (StatusVertice)graph[temp.GetHashCode()];
                if ( adjacent.distance <= curr.distance + 1 ) {
                    continue;
                }

                adjacent.distance = curr.distance + 1;
                adjacent.nextVertice = curr;

                if ( !adjacent.arrived && adjacent.IsValid() ) {
                    adjacent.arrived = true;
                    verticeQueue.Enqueue( adjacent );
                }

            }

            verticeQueue.Dequeue();

        }
    }

    public StatusVertice GetStatus( int priestNum, int devilNum, bool fromBank )
    {
        return ( StatusVertice )graph[StatusVertice.GetHashCodeWithData( priestNum, devilNum, fromBank )];
    }
}
