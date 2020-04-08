using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Interface that adds an index for the BinaryHeap.
/// </summary>
public interface IHeapable
{
	/// <summary>
	/// Index inside the binary tree.
	/// </summary>
	int Index { get; set; }
}
