/********************************************************
 * net8 migration: managed replacement for the WinForms System.Windows.Forms.TreeNode
 * base (and its Nodes collection) used by VTreeNode/MTree. Lets the tree MODEL run headless
 * on .NET 8 — no System.Windows.Forms. UI rendering (if any) lives in the UI layer.
 ********************************************************/
using System;
using System.Collections;
using System.Collections.Generic;

namespace VAdvantage.Classes
{
    /// <summary>Headless stand-in for WinForms TreeNode: just the tree-data surface VA uses.</summary>
    public abstract class VTreeNodeBase
    {
        private VTreeNodeCollection _nodes;

        /// <summary>Display text (WinForms TreeNode.Text).</summary>
        public string Text { get; set; }

        /// <summary>Key used by Nodes.Find (WinForms TreeNode.Name).</summary>
        public string Name { get; set; }

        /// <summary>Arbitrary payload (WinForms TreeNode.Tag).</summary>
        public object Tag { get; set; }

        /// <summary>Presentation hints carried by the model (WinForms TreeNode equivalents);
        /// plain data holders on the headless side, consumed by a UI layer if present.</summary>
        public string ImageKey { get; set; }
        public string SelectedImageKey { get; set; }
        public string ToolTipText { get; set; }

        /// <summary>Parent node (WinForms TreeNode.Parent); null at the root.</summary>
        internal VTreeNode ParentNode;
        public VTreeNode Parent { get { return ParentNode; } }

        /// <summary>Depth in the tree (WinForms TreeNode.Level): root = 0.</summary>
        public int Level { get { return ParentNode == null ? 0 : ParentNode.Level + 1; } }

        /// <summary>First child, or null (WinForms TreeNode.FirstNode).</summary>
        public VTreeNode FirstNode { get { return Nodes.Count > 0 ? Nodes[0] : null; } }

        /// <summary>Child nodes (WinForms TreeNode.Nodes).</summary>
        public VTreeNodeCollection Nodes
        {
            get { return _nodes ?? (_nodes = new VTreeNodeCollection(this)); }
        }
    }

    /// <summary>Headless stand-in for WinForms TreeNodeCollection (the members VA calls).</summary>
    public class VTreeNodeCollection : IEnumerable
    {
        private readonly List<VTreeNode> _items = new List<VTreeNode>();
        private readonly VTreeNodeBase _owner;

        public VTreeNodeCollection() { }
        public VTreeNodeCollection(VTreeNodeBase owner) { _owner = owner; }

        public int Count { get { return _items.Count; } }

        public VTreeNode this[int index]
        {
            get { return _items[index]; }
            set { _items[index] = value; }
        }

        public VTreeNode Add(VTreeNode node) { if (node != null) node.ParentNode = _owner as VTreeNode; _items.Add(node); return node; }
        public void AddRange(VTreeNode[] nodes)
        {
            if (nodes == null) return;
            foreach (var n in nodes) { if (n != null) n.ParentNode = _owner as VTreeNode; }
            _items.AddRange(nodes);
        }
        public void Clear() { _items.Clear(); }
        public void CopyTo(VTreeNode[] array, int index) { _items.CopyTo(array, index); }
        public void Remove(VTreeNode node) { _items.Remove(node); }

        /// <summary>WinForms TreeNodeCollection.Find(key, searchAllChildren) — matches by Name.</summary>
        public VTreeNode[] Find(string key, bool searchAllChildren)
        {
            var result = new List<VTreeNode>();
            Collect(this, key, searchAllChildren, result);
            return result.ToArray();
        }

        private static void Collect(VTreeNodeCollection coll, string key, bool deep, List<VTreeNode> result)
        {
            foreach (VTreeNode n in coll._items)
            {
                if (string.Equals(n.Name, key, StringComparison.OrdinalIgnoreCase))
                    result.Add(n);
                if (deep)
                    Collect(n.Nodes, key, true, result);
            }
        }

        public IEnumerator GetEnumerator() { return _items.GetEnumerator(); }
    }
}
