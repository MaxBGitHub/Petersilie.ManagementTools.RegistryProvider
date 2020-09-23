using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Petersilie.ManagementTools.RegistryProvider
{
	internal class TreeNode<T> : IEnumerable<TreeNode<T>>
	{
		public T Data								{ get; set; }
		public TreeNode<T> Parent					{ get; set; }
		public ICollection<TreeNode<T>> Children	{ get; set; }

		private ICollection<TreeNode<T>> _searchElements 
			= new LinkedList<TreeNode<T>>();


		public bool IsRoot
		{
			get {
				return Parent == null;
			}
		}


		public bool IsLeaf
		{
			get {
				return Children.Count == 0;
			}
		}


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


		public TreeNode<T> Find(Func<TreeNode<T>, bool> predicate)
		{
			return _searchElements.FirstOrDefault(predicate);
		}


		private void RegisterSearchElem(TreeNode<T> elem)
		{
			_searchElements.Add(elem);
			if (Parent != null) {
				Parent.RegisterSearchElem(elem);
			}
		}


		public TreeNode<T> AddChild(T child)
		{
			TreeNode<T> childNode = new TreeNode<T>(child);
			childNode.Parent = this;

			Children.Add(childNode);

			RegisterSearchElem(childNode);

			return childNode;
		}


		public void AddChild(TreeNode<T> child)
		{
			child.Parent = this;

			Children.Add(child);

			RegisterSearchElem(child);
		}


		public override string ToString()
		{
			return Data != null 
				? Data.ToString() 
				: string.Empty;
		}


		public IEnumerator<TreeNode<T>> GetEnumerator()
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


		public TreeNode(T data)
		{
			Data = data;
			Children = new LinkedList<TreeNode<T>>();

			_searchElements.Add(this);
		}

		
	}
}
