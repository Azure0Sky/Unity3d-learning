# README

本次作业实现 P&D 过河游戏智能帮助。

---

- **演示**
![demo](https://github.com/Azure0Sky/Unity3d-learning/blob/master/HW9/PnD%20Helper/demo.gif)

---

- **状态图**

类`StatusGraph`表示游戏的状态图，图数据为该类的`HashTable`类型的数据成员，可加快查找一个状态节点的速度，其value项为类`StatusVertice`，表示一个状态。  

`StatusVertice`类定义如下：
```c#
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
```
  
使用BFS，从原始状态出发动态生成每一个状态节点，将符合要求的节点加入图中，最后生成状态图。  

**状态图自动生成**的代码如下：
```c#
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
```

- 寻找下一步

在生成图之后，使用Dijkstra算法计算每一个状态到达目标状态的路径。  

代码如下：
```c#
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
```