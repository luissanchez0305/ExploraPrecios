using System;

namespace NHCustomProviders
{
	/// <summary>Class that contains the information to map the role provider.</summary>
	public class RoleClassMappingInfo
	{
		#region Properties

		/// <summary>Name of the class to map.</summary>
		public string ClassName { get; set; }

		/// <summary>Name of the table mapped associated to the class.</summary>
		public string TableName { get; set; }

		/// <summary>Name of the id property.</summary>
		public string IdPropertyName { get; set; }

		/// <summary>Column name of the id column.</summary>
		public string IdColumnName { get; set; }

		/// <summary>Type of the id column.</summary>
		public string IdColumnType { get; set; }

		/// <summary>The generator strategy for the id.</summary>
		public string GeneratorStrategy { get; set; }

		/// <summary>Name of the Name property.</summary>
		public string PropName { get; set; }

		/// <summary>Column name the name property.</summary>
		public string PropColumnName { get; set; }

		/// <summary>Indicates if the name property is unique.</summary>
		public bool PropUnique { get; set; }

		/// <summary>Name of the collection property.</summary>
		public string CollectionPropertyName { get; set; }

		/// <summary>Name of the join table.</summary>
		public string JoinTableName { get; set; }

		/// <summary>Set to true to mark this relationship as the inverse side.</summary>
		public bool Inverse { get; set; }

		/// <summary>Name of the associated column for the many-to-many relationship.</summary>
		public string AssociatedColumnName { get; set; }

		/// <summary>Name of the associated class for the many-to-many relationship.</summary>
		public string AssociatedClassName { get; set; }

		#endregion
	}
}