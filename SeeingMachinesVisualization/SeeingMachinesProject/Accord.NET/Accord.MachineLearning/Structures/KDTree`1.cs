﻿// Accord Machine Learning Library
// The Accord.NET Framework
// http://accord.googlecode.com
//
// Copyright © César Souza, 2009-2013
// cesarsouza at gmail.com
//
//    This library is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Lesser General Public
//    License as published by the Free Software Foundation; either
//    version 2.1 of the License, or (at your option) any later version.
//
//    This library is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.
//
//    You should have received a copy of the GNU Lesser General Public
//    License along with this library; if not, write to the Free Software
//    Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
//

namespace Accord.MachineLearning.Structures
{
    using System;
    using System.Collections.Generic;
    using Accord.Math;
    using Accord.Math.Comparers;

    /// <summary>
    ///   K-dimensional tree.
    /// </summary>
    /// 
    /// <typeparam name="T">The type of the value being stored.</typeparam>
    /// 
    /// <remarks>
    /// <para>
    ///   A k-d tree (short for k-dimensional tree) is a space-partitioning data structure 
    ///   for organizing points in a k-dimensional space. k-d trees are a useful data structure
    ///   for several applications, such as searches involving a multidimensional search key 
    ///   (e.g. range searches and nearest neighbor searches). k-d trees are a special case 
    ///   of binary space partitioning trees.</para>
    ///   
    /// <para>
    ///   The k-d tree is a binary tree in which every node is a k-dimensional point. Every non-
    ///   leaf node can be thought of as implicitly generating a splitting hyperplane that divides
    ///   the space into two parts, known as half-spaces. Points to the left of this hyperplane 
    ///   represent the left subtree of that node and points right of the hyperplane are represented
    ///   by the right subtree. The hyperplane direction is chosen in the following way: every node 
    ///   in the tree is associated with one of the k-dimensions, with the hyperplane perpendicular 
    ///   to that dimension's axis. So, for example, if for a particular split the "x" axis is chosen,
    ///   all points in the subtree with a smaller "x" value than the node will appear in the left 
    ///   subtree and all points with larger "x" value will be in the right subtree. In such a case, 
    ///   the hyperplane would be set by the x-value of the point, and its normal would be the unit 
    ///   x-axis.</para>
    /// 
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description>
    ///       Wikipedia, The Free Encyclopedia. K-d tree. Available on:
    ///       http://en.wikipedia.org/wiki/K-d_tree </description></item>
    ///     <item><description>
    ///       Moore, Andrew W. "An intoductory tutorial on kd-trees." (1991).
    ///       Available at: http://www.autonlab.org/autonweb/14665/version/2/part/5/data/moore-tutorial.pdf </description></item>
    ///   </list></para>
    /// </remarks>
    /// 
    /// <example>
    /// <code>
    /// // This is the same example found in Wikipedia page on
    /// // k-d trees: http://en.wikipedia.org/wiki/K-d_tree
    /// 
    /// // Suppose we have the following set of points:
    /// 
    /// double[][] points =
    /// {
    ///     new double[] { 2, 3 },
    ///     new double[] { 5, 4 },
    ///     new double[] { 9, 6 },
    ///     new double[] { 4, 7 },
    ///     new double[] { 8, 1 },
    ///     new double[] { 7, 2 },
    /// };
    /// 
    /// 
    /// // To create a tree from a set of points, we use
    /// KDTree&lt;int> tree = KDTree.FromData&lt;int>(points);
    /// 
    /// // Now we can manually navigate the tree
    /// KDTreeNode&lt;int> node = tree.Root.Left.Right;
    /// 
    /// // Or traverse it automatically
    /// foreach (KDTreeNode&lt;int> n in tree)
    /// {
    ///     double[] location = n.Position;
    ///     Assert.AreEqual(2, location.Length);
    /// }
    /// 
    /// // Given a query point, we can also query for other
    /// // points which are near this point within a radius
    /// 
    /// double[] query = new double[] { 5, 3 };
    /// 
    /// // Locate all nearby points within an euclidean distance of 1.5
    /// // (answer should be be a single point located at position (5,4))
    /// KDTreeNodeCollection&lt;int> result = tree.Nearest(query, radius: 1.5); 
    ///             
    /// // We can also use alternate distance functions
    /// tree.Distance = Accord.Math.Distance.Manhattan;
    /// 
    /// // And also query for a fixed number of neighbor points
    /// // (answer should be the points at (5,4), (7,2), (2,3))
    /// KDTreeNodeCollection&lt;int> neighbors = tree.Nearest(query, neighbors: 3);
    /// </code>
    /// </example>
    /// 
    /// <seealso cref="KNearestNeighbors"/>
    /// 
    [Serializable]
    public class KDTree<T> : IEnumerable<KDTreeNode<T>>
    {

        private int count;
        private int dimensions;
        private KDTreeNode<T> root;
        private Func<double[], double[], double> distance;


        /// <summary>
        ///   Gets the root of the tree.
        /// </summary>
        /// 
        public KDTreeNode<T> Root
        {
            get { return root; }
        }

        /// <summary>
        ///   Gets the number of dimensions expected
        ///   by the input points of this tree.
        /// </summary>
        /// 
        public int Dimensions
        {
            get { return dimensions; }
        }

        /// <summary>
        ///   Gets or set the distance function used to
        ///   measure distances amongst points on this tree
        /// </summary>
        /// 
        public Func<double[], double[], double> Distance
        {
            get { return distance; }
            set { distance = value; }
        }

        /// <summary>
        ///   Gets the number of elements contained in this
        ///   tree. This is also the number of tree nodes.
        /// </summary>
        /// 
        public int Count
        {
            get { return count; }
        }


        /// <summary>
        ///   Creates a new <see cref="KDTree&lt;T&gt;"/>.
        /// </summary>
        /// 
        /// <param name="dimensions">The number of dimensions in the tree.</param>
        /// 
        public KDTree(int dimensions)
        {
            this.dimensions = dimensions;
            this.distance = Accord.Math.Distance.SquareEuclidean;
        }

        /// <summary>
        ///   Creates a new <see cref="KDTree&lt;T&gt;"/>.
        /// </summary>
        /// 
        /// <param name="dimension">The number of dimensions in the tree.</param>
        /// <param name="root">The root node, if already existent.</param>
        /// 
        public KDTree(int dimension, KDTreeNode<T> root)
            : this(dimension)
        {
            this.root = root;
        }


        /// <summary>
        ///   Inserts a value into the tree at the desired position.
        /// </summary>
        /// 
        /// <param name="position">A double-vector with the same number of elements as dimensions in the tree.</param>
        /// <param name="value">The value to be added.</param>
        /// 
        public void Add(double[] position, T value)
        {
            insert(ref root, position, value, 0);
        }

        /// <summary>
        ///   Retrieves the nearest points to a given point within a given radius.
        /// </summary>
        /// 
        /// <param name="position">The queried point.</param>
        /// <param name="radius">The search radius.</param>
        /// 
        /// <returns>A list of neighbor points, ordered by distance.</returns>
        /// 
        public KDTreeNodeCollection<T> Nearest(double[] position, double radius)
        {
            var list = new KDTreeNodeCollection<T>();
            if (root != null) find(root, position, radius, list);
            return list;
        }

        /// <summary>
        ///   Retrieves a fixed point of nearest points to a given point.
        /// </summary>
        /// 
        /// <param name="position">The queried point.</param>
        /// <param name="neighbors">The number of neighbors to retrieve.</param>
        /// 
        /// <returns>A list of neighbor points, ordered by distance.</returns>
        /// 
        public KDTreeNodeCollection<T> Nearest(double[] position, int neighbors)
        {
            var list = new KDTreeNodeCollection<T>(size: neighbors);
            if (root != null) find(root, position, neighbors, list);
            return list;
        }


        #region internal methods
        internal static KDTree<T> FromData(double[][] points)
        {
            return KDTree<T>.FromData(points, null, Accord.Math.Distance.Euclidean);
        }

        internal static KDTree<T> FromData(double[][] points, T[] values)
        {
            return KDTree<T>.FromData(points, values, Accord.Math.Distance.Euclidean);
        }

        internal static KDTree<T> FromData(double[][] points, Func<double[], double[], double> distance)
        {
            return KDTree<T>.FromData(points, null, distance);
        }

        internal static KDTree<T> FromData(double[][] points, T[] values, Func<double[], double[], double> distance)
        {
            // Initial argument checks for creating the tree
            if (points == null) throw new ArgumentNullException("points");
            if (distance == null) throw new ArgumentNullException("distance");
            if (values != null && points.Length != values.Length)
                throw new DimensionMismatchException("values");

            int dimensions = points[0].Length;

            // Create a comparer to compare individual array
            // elements at specified positions when sorting
            ElementComparer comparer = new ElementComparer();

            // Since all sorting will be done in-place, we
            // will register the original order of values
            int[] idx = Matrix.Indices(0, points.Length);

            // Call the recursive algorithm to operate on the whole array (from 0 to points.Length)
            KDTreeNode<T> root = create(points, idx, values, 0, dimensions, 0, points.Length, comparer);

            // Restore the original ordering
            Array.Sort(idx, points);

            // Create and return the newly formed tree
            KDTree<T> tree = new KDTree<T>(dimensions);
            tree.root = root;
            tree.count = points.Length;
            tree.distance = distance;
            return tree;
        }
        #endregion


        #region Recursive methods
        private void find(KDTreeNode<T> current, double[] position, double radius, KDTreeNodeCollection<T> list)
        {
            // Check if the distance of the point from this
            // node is within the desired radius, and if it
            // is, add to the list of nearest nodes.

            double d = distance(position, current.Position);

            if (d <= radius)
                list.TryAdd(current, d);

            // Prepare for recursion. The following null checks
            // will be used to avoid function calls if possible

            double value = position[current.Axis];
            double median = current.Position[current.Axis];

            if (value < median)
            {
                if (current.Left != null)
                    find(current.Left, position, radius, list);

                if (current.Right != null)
                    if (Math.Abs(value - median) <= radius)
                        find(current.Right, position, radius, list);
            }
            else
            {
                if (current.Right != null)
                    find(current.Right, position, radius, list);

                if (current.Left != null)
                    if (Math.Abs(value - median) <= radius)
                        find(current.Left, position, radius, list);
            }
        }

        private void find(KDTreeNode<T> current, double[] position, int neighbors, KDTreeNodeCollection<T> list)
        {
            // Compute distance from this node to the point
            double d = distance(position, current.Position);

            if (current.IsLeaf)
            {
                // Base: node is a leaf
                list.TryAdd(current, d);
            }
            else
            {
                // Check for leafs on the opposite sides of 
                // the subtrees to find possible neighbors.

                // Prepare for recursion. The following null checks
                // will be used to avoid function calls if possible

                double value = position[current.Axis];
                double median = current.Position[current.Axis];

                if (value < median)
                {
                    if (current.Left != null)
                        find(current.Left, position, neighbors, list);

                    list.TryAdd(current, d);

                    if (current.Right != null)
                        if (Math.Abs(value - median) <= list.Farthest.Distance)
                            find(current.Right, position, neighbors, list);
                }
                else
                {
                    if (current.Right != null)
                        find(current.Right, position, neighbors, list);

                    list.TryAdd(current, d);

                    if (current.Left != null)
                        if (Math.Abs(value - median) <= list.Farthest.Distance)
                            find(current.Left, position, neighbors, list);
                }
            }
        }


        private void insert(ref KDTreeNode<T> node, double[] position, T value, int depth)
        {
            if (node == null)
            {
                // Base case: node is null
                node = new KDTreeNode<T>()
                {
                    Axis = depth % dimensions,
                    Position = position,
                    Value = value
                };
            }
            else
            {
                // Recursive case: keep looking for a position to insert

                int newIndex = depth % dimensions; // with this, each depth of 
                // the tree will operate on one of the dimensions of the data

                if (position[node.Axis] < node.Position[node.Axis])
                {
                    KDTreeNode<T> child = node.Left;
                    insert(ref child, position, value, depth + 1);
                    node.Left = child;
                }
                else
                {
                    KDTreeNode<T> child = node.Right;
                    insert(ref child, position, value, depth + 1);
                    node.Right = child;
                }
            }
        }


        private static KDTreeNode<T> create(double[][] points, int[] idx, T[] values,
           int depth, int k, int start, int length, ElementComparer comparer)
        {
            if (length <= 0)
                return null;

            // We will be doing sorting in place
            int axis = comparer.Index = depth % k;
            Array.Sort(points, idx, start, length, comparer);

            // Middle of the input section
            int half = start + length / 2;

            // Start and end of the left branch
            int leftStart = start;
            int leftLength = half - start;

            // Start and end of the right branch
            int rightStart = half + 1;
            int rightLength = length - length / 2 - 1;

            // The median will be located halfway in the sorted array
            var median = points[half];
            var value = values != null ? values[idx[half]] : default(T);

            depth++;

            // Continue with the recursion, passing the appropriate left and right array sections
            KDTreeNode<T> left = create(points, idx, values, depth, k, leftStart, leftLength, comparer);
            KDTreeNode<T> right = create(points, idx, values, depth, k, rightStart, rightLength, comparer);

            // Backtrack and create
            return new KDTreeNode<T>()
            {
                Axis = axis,
                Position = median,
                Value = value,
                Left = left,
                Right = right,
            };
        }
        #endregion


        /// <summary>
        ///   Removes all nodes from this tree.
        /// </summary>
        /// 
        public void Clear()
        {
            this.root = null;
        }

        /// <summary>
        ///   Copies the entire tree to a compatible one-dimensional <see cref="System.Array"/>, starting
        ///   at the specified <paramref name="arrayIndex">index</paramref> of the <paramref name="array">target array</paramref>.
        /// </summary>
        /// 
        /// <param name="array">The one-dimensional <see cref="System.Array"/> that is the destination of the
        ///    elements copied from tree. The <see cref="System.Array"/> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        /// 
        public void CopyTo(KDTreeNode<T>[] array, int arrayIndex)
        {
            foreach (var node in this)
            {
                array[arrayIndex++] = node;
            }
        }

        /// <summary>
        ///   Returns an enumerator that iterates through the tree.
        /// </summary>
        /// 
        /// <returns>
        ///   An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        /// 
        public IEnumerator<KDTreeNode<T>> GetEnumerator()
        {
            if (root == null)
                yield break;

            var stack = new Stack<KDTreeNode<T>>(new[] { root });

            while (stack.Count != 0)
            {
                KDTreeNode<T> current = stack.Pop();

                yield return current;

                if (current.Left != null)
                    stack.Push(current.Left);

                if (current.Right != null)
                    stack.Push(current.Right);
            }
        }

        /// <summary>
        ///   Traverse the tree using a <see cref="KDTreeTraversal">tree traversal
        ///   method</see>. Can be iterated with a foreach loop.
        /// </summary>
        /// 
        /// <param name="method">The tree traversal method. Common methods are
        /// available in the <see cref="KDTreeTraversal"/>static class.</param>
        /// 
        /// <returns>An <see cref="IEnumerable{T}"/> object which can be used to
        /// traverse the tree using the chosen traversal method.</returns>
        /// 
        public IEnumerable<KDTreeNode<T>> Traverse(KDTreeTraversalMethod<T> method)
        {
            return new KDTreeTraversal(this, method);
        }


        /// <summary>
        ///   Returns an enumerator that iterates through the tree.
        /// </summary>
        /// 
        /// <returns>
        ///   An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        /// 
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }


        private class KDTreeTraversal : IEnumerable<KDTreeNode<T>>
        {

            private KDTree<T> tree;
            private KDTreeTraversalMethod<T> method;

            public KDTreeTraversal(KDTree<T> tree, KDTreeTraversalMethod<T> method)
            {
                this.tree = tree;
                this.method = method;
            }

            public IEnumerator<KDTreeNode<T>> GetEnumerator()
            {
                return method(tree);
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return method(tree);
            }
        }
    }
}
