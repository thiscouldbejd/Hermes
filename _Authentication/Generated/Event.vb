Namespace Authentication

	''' <summary></summary>
	''' <autogenerated>Generated from a T4 template. Modifications will be lost, if applicable use a partial class instead.</autogenerated>
	''' <generator-date>01/02/2014 12:15:11</generator-date>
	''' <generator-functions>1</generator-functions>
	''' <generator-source>Hermes_\_Authentication\Generated\Event.tt</generator-source>
	''' <generator-template>Text-Templates\Classes\VB_Object.tt</generator-template>
	''' <generator-version>1</generator-version>
	<System.CodeDom.Compiler.GeneratedCode("Hermes_\_Authentication\Generated\Event.tt", "1")> _
	Partial Public Class [Event]
		Inherits System.Object

		#Region " Public Constructors "

			''' <summary>Default Constructor</summary>
			Public Sub New()

				MyBase.New()

				m_Created = DateTime.Now()
			End Sub

			''' <summary>Parametered Constructor (1 Parameters)</summary>
			Public Sub New( _
				ByVal _Name As System.String _
			)

				MyBase.New()

				Name = _Name

				m_Created = DateTime.Now()
			End Sub

			''' <summary>Parametered Constructor (2 Parameters)</summary>
			Public Sub New( _
				ByVal _Name As System.String, _
				ByVal _Details As System.String _
			)

				MyBase.New()

				Name = _Name
				Details = _Details

				m_Created = DateTime.Now()
			End Sub

			''' <summary>Parametered Constructor (3 Parameters)</summary>
			Public Sub New( _
				ByVal _Name As System.String, _
				ByVal _Details As System.String, _
				ByVal _Created As System.DateTime _
			)

				MyBase.New()

				Name = _Name
				Details = _Details
				m_Created = _Created

			End Sub

		#End Region

		#Region " Public Constants "

			''' <summary>Public Shared Reference to the Name of the Property: Name</summary>
			''' <remarks></remarks>
			Public Const PROPERTY_NAME As String = "Name"

			''' <summary>Public Shared Reference to the Name of the Property: Details</summary>
			''' <remarks></remarks>
			Public Const PROPERTY_DETAILS As String = "Details"

			''' <summary>Public Shared Reference to the Name of the Property: Created</summary>
			''' <remarks></remarks>
			Public Const PROPERTY_CREATED As String = "Created"

		#End Region

		#Region " Private Variables "

			''' <summary>Private Data Storage Variable for Property: Name</summary>
			''' <remarks></remarks>
			Private m_Name As System.String

			''' <summary>Private Data Storage Variable for Property: Details</summary>
			''' <remarks></remarks>
			Private m_Details As System.String

			''' <summary>Private Data Storage Variable for Property: Created</summary>
			''' <remarks></remarks>
			Private m_Created As System.DateTime

		#End Region

		#Region " Public Properties "

			''' <summary>Provides Access to the Property: Name</summary>
			''' <remarks></remarks>
			Public Property Name() As System.String
				Get
					Return m_Name
				End Get
				Set(value As System.String)
					m_Name = value
				End Set
			End Property

			''' <summary>Provides Access to the Property: Details</summary>
			''' <remarks></remarks>
			Public Property Details() As System.String
				Get
					Return m_Details
				End Get
				Set(value As System.String)
					m_Details = value
				End Set
			End Property

			''' <summary>Provides Access to the Property: Created</summary>
			''' <remarks></remarks>
			Public ReadOnly Property Created() As System.DateTime
				Get
					Return m_Created
				End Get
			End Property

		#End Region

	End Class

End Namespace