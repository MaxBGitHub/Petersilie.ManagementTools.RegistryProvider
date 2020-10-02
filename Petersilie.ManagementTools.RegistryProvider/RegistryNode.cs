using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Petersilie.ManagementTools.RegistryProvider
{
    public class RegistryNode : IEnumerable<RegistryNode>
    {
        /// <summary>
        /// The Registry tree, also known as hive.
        /// </summary>
        public RegHive Hive { get; private set; }
        /// <summary>
        /// The Registry in which to move.
        /// </summary>
        public RegistryBase Registry { get; private set; }
        /// <summary>
        /// The key that is stored in this Node.
        /// </summary>
        public string Key { get; private set; }
        /// <summary>
        /// The parent node.
        /// </summary>
        public RegistryNode Parent { get; set; }
        /// <summary>
        /// Child nodes.
        /// </summary>
        public ICollection<RegistryNode> Children { get; set; }

        private ICollection<RegistryNode> _searchElements 
            = new LinkedList<RegistryNode>();


        /// <summary>
        /// True if this node has no parent.
        /// </summary>
        public bool IsRoot
        {
            get {
                return Parent == null;
            }
        }


        /// <summary>
        /// True if this node has no children.
        /// </summary>
        public bool IsLeaf
        {
            get {
                return Children.Count == 0;
            }
        }


        /// <summary>
        /// Depth or level of the node.
        /// </summary>
        public int Level
        {
            get
            {
                if (IsRoot) {
                    return 0;
                }
                else {
                    return Parent.Level + 1;
                }                
            }
        }


        /// <summary>
        /// The complete key path.
        /// </summary>
        public string Path
        {
            get
            {
                if (this.IsRoot) {
                    return Key;
                }
                else
                {
                    string[] path = new string[Level];
                    int i = Level;
                    var bee = this;
                    while (bee.Parent != null) {
                        path[--i] = bee.Key;
                        bee = bee.Parent;
                    }
                    return string.Join("\\", path);
                }
            }
        }


        /// <summary>
        /// Tree traversal implementation
        /// </summary>
        /// <param name="action"></param>
        public void Traverse(Action<RegistryNode> action)
        {
            action(this);
            foreach (var child in Children) {
                child.Traverse(action);
            }
        }


        /// <summary>
        /// Converts the tree structure into a flat collection of your choice
        /// </summary>
        /// <returns></returns>
        public IEnumerable<RegistryNode> Flatten()
        {
            return new[] { this }.Concat(Children.SelectMany(bee => bee.Flatten()));
        }


        private void BuildHiveInternal(RegistryNode bee, int depth)
        {
            string path = bee.Key;
            var queenBee = bee;
            while (queenBee.Parent != null) {                
                queenBee = queenBee.Parent;         
                if ( !(string.IsNullOrEmpty(queenBee.Key)) ) {
                    path = queenBee.Key + "\\" + path;
                }                
            }

            bool hasAccess;
            bee.Registry.HasPermission( bee.Hive, 
                                        path, 
                                        RegAccessFlags.QueryValue, 
                                        out hasAccess);
            if (!hasAccess) {
                return;
            }

            var honeyCombs = bee.Registry.GetSubKeys(bee.Hive, path, false);
            Parallel.ForEach(honeyCombs, honeyComb => {
                RegistryNode larvae = bee.AddChild(honeyComb);
                if (larvae.Level < depth || depth == -1) {
                    BuildHiveInternal(larvae, depth);
                }
            });
            //foreach (var honeyComb in honeyCombs) {
            //    RegistryBee larvae = bee.AddChild(honeyComb);
            //    if (larvae.Level < depth || depth == -1) {
            //        BuildHiveInternal(larvae, depth);
            //    }
            //}
        }


        /// <summary>
        /// Builds the tree structure up to the specified depth.
        /// </summary>
        /// <param name="depth">-1 to ignore depth</param>
        public void BuildHive(int depth)
        {
            BuildHiveInternal(this, depth);
        }

        /// <summary>
        /// Builds the tree structure.
        /// </summary>
        public void BuildHive()
        {
            BuildHiveInternal(this, -1);
        }


        /// <summary>
        /// Looks for the specified RegistryNode.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public RegistryNode Find(Func<RegistryNode, bool> predicate)
        {
            return _searchElements.FirstOrDefault(predicate);
        }


        private void RegisterSearchElem(RegistryNode elem)
        {
            if (elem == null) {
                return;
            }

            _searchElements.Add(elem);
            if (Parent != null) {
                Parent.RegisterSearchElem(elem);
            }
        }


        /// <summary>
        /// Add a new child node.
        /// </summary>
        /// <param name="key">The key of the child.</param>
        /// <returns></returns>
        public RegistryNode AddChild(string key)
        {
            RegistryNode larvae = new RegistryNode(Registry, Hive);
            larvae.Key = key;
            larvae.Parent = this;
            Children.Add(larvae);
            RegisterSearchElem(larvae);

            return larvae;
        }

        /// <summary>
        /// Adds the existing node as child.
        /// </summary>
        /// <param name="child"></param>
        public void AddChild(RegistryNode child)
        {
            if (child.Hive != this.Hive) {
                throw new ArgumentException("Hive of child must be the same.");
            }

            child.Parent = this;
            child.Registry = this.Registry;

            Children.Add(child);

            RegisterSearchElem(child);
        }


        public override string ToString()
        {
            return Key != null 
                ? Key.ToString() 
                : string.Empty;
        }


        public IEnumerator<RegistryNode> GetEnumerator()
        {
            yield return this;
            foreach (var child in Children) {
                foreach (var grandChildren in child.Children) {
                    yield return grandChildren;
                }
            }
        }


        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }


        /// <summary>
        /// Initializes a RegistryNode with the given key.
        /// The key must follow the standard registry 
        /// key path contraints.
        /// </summary>
        /// <param name="view">64-bit or 32-bit</param>
        /// <param name="hive">Registry tree, also kown as hive</param>
        /// <param name="key">The complete key path.</param>
        public RegistryNode(RegView view, RegHive hive, string key)
        {
            Hive = hive;
            Key = key;
            if (view == RegView.x64) {
                Registry = new Registry64();
            } else {
                Registry = new Registry32();
            }                        

            Children = new LinkedList<RegistryNode>();
            _searchElements.Add(this);
        }


        /// <summary>
        /// Initiiazles a new RegistryNode
        /// </summary>
        /// <param name="view"></param>
        /// <param name="hive"></param>
        public RegistryNode(RegView view, RegHive hive)
        {
            Hive = hive;
            Key = string.Empty;
            if (view == RegView.x64) {
                Registry = new Registry64();
            } else {
                Registry = new Registry32();
            }                        

            Children = new LinkedList<RegistryNode>();
            _searchElements.Add(this);
        }


        internal RegistryNode(RegistryBase registry, RegHive hive)
        {
            Hive = hive;
            Key = string.Empty;
            Registry = registry;

            Children = new LinkedList<RegistryNode>();
            _searchElements.Add(this);
        }
    }
}
