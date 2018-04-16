using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TestSortedList : MonoBehaviour {

    struct AstarVoxel
    {
        public float priority;
        public Vector2Int index;

        public AstarVoxel(float p, Vector2Int i)
        {
            priority = p;
            index = i;
        }
    }

    struct Pet
    {
        public string Name;
        public int Age;
    }

	// Use this for initialization
	void Start () {
        /*
        SortedList<float, Vector2Int> frontier = new SortedList<float, Vector2Int>();
       
        frontier.Add(0.3f, new Vector2Int(1, 1));
        frontier.Add(1f, new Vector2Int(1, 2));
        frontier.Add(0.5f, new Vector2Int(0, 2));

        print(frontier.Count);

        Vector2Int pop = frontier.GetByIndex(1);
        */

        /*
        SortedList<float, int> test = new SortedList<float, int>();
        test.Add(0.2f, 100);
        test.Add(0.1f, 200);
        test.Add(0.1f, 150);

        float key = test.Keys[0];
        int value = test.Values[0];
        print(key);
        print(value);

        test.RemoveAt(0);
        print(test.Keys[0]);
        print(test.Values[0]);
        */

        /*
        ArrayList test = new ArrayList();
        test.Add(1);
        test.Add(2);
        test.Reverse();

        int[] fromArray = (int[])test.ToArray(typeof(int));
        for (int i = 0; i < test.Count; ++i)
        {
            int element = fromArray[i];
            print(element);
        }
        */

        List<AstarVoxel> test = new List<AstarVoxel>();
        test.Add(new AstarVoxel(1.0f, new Vector2Int(1, 2)));
        test.Add(new AstarVoxel(0.5f, new Vector2Int(2, 2)));
        test.Add(new AstarVoxel(1.0f, new Vector2Int(4, 4)));

        List<AstarVoxel> sorted = test.OrderBy(o => o.priority).ToList<AstarVoxel>();

        for (int i = 0; i < sorted.Count; ++i)
        {
            print(sorted[i].index);
        }
    }
}
