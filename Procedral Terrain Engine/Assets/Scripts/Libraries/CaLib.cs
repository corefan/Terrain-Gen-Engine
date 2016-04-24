﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

 namespace cellularAutomataLib
{

	public delegate T1 genericDelegate<T1,T2>(T2 item);

	public class finiteCellularAutomota
	{
		public finiteCellularAutomota(int[,] cells, genericDelegate<int,int>[] rules, Dictionary<int,int[]> ruleDictionary)
		{
			this.cells = cells;
			this.rules = rules;
			this.ruleDictionary = ruleDictionary;
		}

		public int[,] cells;
		public genericDelegate<int,int>[] rules;
		public Dictionary<int,int[]> ruleDictionary;
		public void pass(bool NESW) //if not nesw will asume neswdiag
		{
			Vector2[] neighbors = new Vector2[8];
			for(int y =0; y < cells.GetLength(1); y++)
			{
				for(int x =0; x < cells.GetLength(0); x++)
				{
					if(NESW)
					{
						neighbors = neighborAcces.getNeigborsNESW(ref cells,new Vector2(x,y));
					}
					else
					{
						neighbors = neighborAcces.getNeigborsNESWDiag(ref cells,new Vector2(x,y));
					}

					foreach(Vector2 cellCoord in neighbors)
					{
						genericDelegate<int,int> rule;
						rule = ruleMatrix<int,int>.getRule(cells[(int)cellCoord.x,(int)cellCoord.y],cells[x,y],ref rules,ref ruleDictionary);
						cells[(int)cellCoord.x,(int)cellCoord.y] = rule(cells[x,y]);
					}

				}
			}
		}
	}

	public static class neighborAcces
	{

		public enum direction : int
		{
			UP =0,DOWN =1,LEFT =2,RIGHT =3,UP_LEFT=4,UP_RIGHT=5,DOWN_LEFT=6,DOWN_RIGHT=7
		}

		static readonly Vector2[] directionVectors = new Vector2[8]{
		Vector2.up,Vector2.down,Vector2.left,Vector2.right,
		Vector3.up+Vector3.left,Vector3.up+Vector3.right,
		Vector2.down + Vector2.left, Vector2.down + Vector2.left
		};

		//pointers are needed for this aporach to alow the retured values to be editable

		static public Vector2[]  getNeigborsNESW<T>(ref T[,] array, Vector2 cell) //no flattened array implemenation as of now
		{
			return getNeigbors(ref array,cell,new direction[4]{direction.UP,direction.RIGHT,direction.DOWN,direction.LEFT});
		}
		static public Vector2[]  getNeigborsNESWDiag<T>(ref T[,] array, Vector2 cell) //no flattened array implemenation as of now
		{
			return getNeigbors(ref array,cell,new direction[8]{direction.UP,direction.UP_RIGHT,direction.RIGHT,direction.DOWN_RIGHT,direction.DOWN,direction.DOWN_LEFT,direction.LEFT,direction.UP_LEFT});
		}

		static public Vector2[]  getNeigbors<T>(ref T[,] array, Vector2 cell,direction[] sidesToCheck) //no flattened array implemenation as of now
		{
			Vector2[] cells = new Vector2[sidesToCheck.Length];
			bool[] isNull = new bool[sidesToCheck.Length];
			int legalCells =0;
			int lengthX = array.GetLength(0);
			int lengthY = array.GetLength(1);
			int cellX = (int)cell.x;
			int cellY = (int)cell.y;

			# region checkNulls
			for(int i=0; i < sidesToCheck.Length; i++)
			{
				cellX = (int)cell.x + (int)neighborAcces.directionVectors[(int)sidesToCheck[i]].x;
				cellY = (int)cell.y + (int)neighborAcces.directionVectors[(int)sidesToCheck[i]].y;
				if(cellX >= 0 && cellY >= 0 && cellX < lengthX && cellY < lengthY)
				{
					isNull[i] = false;
					cells[i] = new Vector2(cellX,cellY);
					legalCells ++;
				}
				else
				{
					isNull[i] = true;
				}
			}
			#endregion

			#region buildArray
			Vector2[] finalCells = new Vector2[legalCells];
			int cellsIndex =0;
			for(int i=0; i < sidesToCheck.Length; i++)
			{
				if(isNull[i] == false)
				{
					finalCells[cellsIndex] = cells[i];
					cellsIndex ++;
				}
			}
			#endregion
			return finalCells;
		}
	}
	//t1 is output t2 is input 
	public static class ruleMatrix<T1,T2>
	{
		public static int getRuleNumber(int state,int neighbor,ref genericDelegate<T1,T2>[] rules,ref Dictionary<int,int[]> ruleDictionary)
		{

			if(state < 0 || neighbor < 0 || state > ruleDictionary.Count ||neighbor > ruleDictionary.Count)
			{
				Debug.LogError(string.Format("Invalid state for either cell: {0} or {1} in rule matrix",state,neighbor));
			}

			return(ruleDictionary[state][neighbor]);
		}

		public static genericDelegate<T1,T2> getRule(int state,int neighbor,ref genericDelegate<T1,T2>[] rules,ref Dictionary<int,int[]> ruleDictionary)
		{
			//Debug.Log(state+"  ,  "+neighbor);
			return rules[getRuleNumber(state,neighbor,ref rules,ref ruleDictionary)];
		}
	}	
}
