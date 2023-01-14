using UnityEditor;
using System.Linq;

/// <summary>
/// Siblingを上下に移動させるエディタ拡張
/// </summary>
public class UniSiblingChanger : Editor
{

    /// <summary>
    /// Siblingを上げ、前方に移動する
    /// </summary>
    [MenuItem("GameObject/CustomShortcuts/Set to Next Sibling %#=")]
    static void SetToNextSibling()
    {
        ChangeSibling(1);
    }

    /// <summary>
    /// Siblingを下げ、後方に移動する
    /// </summary>
    [MenuItem("GameObject/CustomShortcuts/Set to Previous Sibling %#-")]
    static void SetToPreviousSibling()
    {
        ChangeSibling(-1);
    }

    /// <summary>
    /// Siblingを移動量変える
    /// </summary>
    /// <param name="count">移動量</param>
    static void ChangeSibling(int count)
    {
        var objects = Selection.gameObjects;
        if(objects == null) {
            return;
        }
        // ソート条件
        var desc = count > 0 ? -1 : 1;
        // 並び替え
        foreach(var obj in objects.OrderBy(x => desc * x.transform.GetSiblingIndex())) {
            var index = obj.transform.GetSiblingIndex();
            obj.transform.SetSiblingIndex(index + count);
        }
    }

    [MenuItem("GameObject/CustomShortcuts/Set to Top %#_[")]
    static void SetToTop()
    {
        MoveToEdge(0);
    }

    [MenuItem("GameObject/CustomShortcuts/Set to Bottom %#_]")]
    static void SetToBottom()
    {
        MoveToEdge(-1);
    }

    static void MoveToEdge(int count)
    {
        var objects = Selection.gameObjects;
        if (objects == null)
        {
            return;
        }
        var desc = count > 0 ? -1 : 1;
        foreach (var obj in objects.OrderBy(x => desc * x.transform.GetSiblingIndex()))
        {
            if(count == 0) obj.transform.SetSiblingIndex(count);
            if (count == -1) obj.transform.SetAsLastSibling();
        }
    }
}
