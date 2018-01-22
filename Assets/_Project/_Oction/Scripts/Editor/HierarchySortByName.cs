using UnityEngine;
using System.Collections;
using UnityEditor;

/// <summary>
/// To use this functionality, look at the Hierarchy panel, at the top right position. 
/// Click the icon located there and it will show this option.
/// </summary>
public class HierarchySortByName : BaseHierarchySort
{
    public override int Compare(GameObject lhs, GameObject rhs)
    {
        if (lhs == rhs) return 0; // If they are the same, then exit 
        if (lhs == null) return -1; // If one or other object is null, then exit 
        if (rhs == null) return 1;
        // Now compare the names of two objects and sort 
        return EditorUtility.NaturalCompare(lhs.name, rhs.name);
    }
}
