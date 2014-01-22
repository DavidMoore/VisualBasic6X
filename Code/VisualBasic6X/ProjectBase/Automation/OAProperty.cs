/* ****************************************************************************
 *
 * Copyright (c) Microsoft Corporation. 
 *
 * This source code is subject to terms and conditions of the Apache License, Version 2.0. A 
 * copy of the license can be found in the License.html file at the root of this distribution. If 
 * you cannot locate the Apache License, Version 2.0, please send an email to 
 * ironpy@microsoft.com. By using this source code in any fashion, you are agreeing to be bound 
 * by the terms of the Apache License, Version 2.0.
 *
 * You must not remove this notice, or any other, from this software.
 *
 * ***************************************************************************/

using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Microsoft.VisualStudio.Project.Automation
{
	[CLSCompliant(false), ComVisible(true)]
	public class OAProperty : EnvDTE.Property
	{
		#region fields
		private OAProperties parent;
		private PropertyInfo pi;
		#endregion

		#region ctors

		public OAProperty(OAProperties parent, PropertyInfo pi)
		{
			this.parent = parent;
			this.pi = pi;
		}
		#endregion

		#region EnvDTE.Property
		/// <summary>
		/// Microsoft Internal Use Only.
		/// </summary>
		public object Application
		{
			get { return null; }
		}

		/// <summary>
		/// Gets the Collection containing the Property object supporting this property.
		/// </summary>
		public EnvDTE.Properties Collection
		{
			get
			{
				//todo: EnvDTE.Property.Collection
				return this.parent;
			}
		}

		/// <summary>
		/// Gets the top-level extensibility object.
		/// </summary>
		public EnvDTE.DTE DTE
		{
			get
			{
				return this.parent.DTE;
			}
		}

		/// <summary>
		/// Returns one element of a list. 
		/// </summary>
		/// <param name="index1">The index of the item to display.</param>
		/// <param name="index2">The index of the item to display. Reserved for future use.</param>
		/// <param name="index3">The index of the item to display. Reserved for future use.</param>
		/// <param name="index4">The index of the item to display. Reserved for future use.</param>
		/// <returns>The value of a property</returns>
		public object get_IndexedValue(object index1, object index2, object index3, object index4)
		{
			ParameterInfo[] par = pi.GetIndexParameters();
			int len = Math.Min(par.Length, 4);
			if(len == 0) return this.Value;
			object[] index = new object[len];
			Array.Copy(new object[4] { index1, index2, index3, index4 }, index, len);
			return this.pi.GetValue(this.parent.Target, index);
		}

		/// <summary>
		/// Setter function to set properties values. 
		/// </summary>
		/// <param name="value"></param>
		public void let_Value(object value)
		{
			this.Value = value;
		}

		/// <summary>
		/// Gets the name of the object.
		/// </summary>
		public string Name
		{
			get
			{
				return pi.Name;
			}
		}

		/// <summary>
		/// Gets the number of indices required to access the value.
		/// </summary>
		public short NumIndices
		{
			get { return (short)pi.GetIndexParameters().Length; }
		}

		/// <summary>
		/// Sets or gets the object supporting the Property object.
		/// </summary>
		public object Object
		{
			get
			{
				return this.parent.Target;
			}
			set
			{
			}
		}

		/// <summary>
		/// Microsoft Internal Use Only.
		/// </summary>
		public EnvDTE.Properties Parent
		{
			get { return this.parent; }
		}

		/// <summary>
		/// Sets the value of the property at the specified index.
		/// </summary>
		/// <param name="index1">The index of the item to set.</param>
		/// <param name="index2">Reserved for future use.</param>
		/// <param name="index3">Reserved for future use.</param>
		/// <param name="index4">Reserved for future use.</param>
		/// <param name="value">The value to set.</param>
		public void set_IndexedValue(object index1, object index2, object index3, object index4, object value)
		{
			ParameterInfo[] par = pi.GetIndexParameters();
			int len = Math.Min(par.Length, 4);
			if(len == 0)
			{
				this.Value = value;
			}
			else
			{
				object[] index = new object[len];
				Array.Copy(new object[4] { index1, index2, index3, index4 }, index, len);

				using(AutomationScope scope = new AutomationScope(this.parent.Target.Node.ProjectMgr.Site))
				{
					this.pi.SetValue(this.parent.Target, value, index);
				}
			}

		}

		/// <summary>
		/// Gets or sets the value of the property returned by the Property object.
		/// </summary>
		public object Value
		{
			get { return pi.GetValue(this.parent.Target, null); }
			set
			{
				using(AutomationScope scope = new AutomationScope(this.parent.Target.Node.ProjectMgr.Site))
				{
					this.pi.SetValue(this.parent.Target, value, null);
				}
			}
		}
		#endregion
	}
}
