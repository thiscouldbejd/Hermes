Namespace Email

	''' <summary></summary>
	''' <autogenerated>Generated from a T4 template. Modifications will be lost, if applicable use a partial class instead.</autogenerated>
	''' <generator-date>01/02/2014 12:16:01</generator-date>
	''' <generator-functions>1</generator-functions>
	''' <generator-source>Hermes_\_Email\Generated\Distribution.tt</generator-source>
	''' <generator-template>Text-Templates\Classes\VB_Object.tt</generator-template>
	''' <generator-version>1</generator-version>
	<System.CodeDom.Compiler.GeneratedCode("Hermes_\_Email\Generated\Distribution.tt", "1")> _
	Partial Public Class Distribution
		Inherits System.Object

		#Region " Public Constructors "

			''' <summary>Default Constructor</summary>
			Public Sub New()

				MyBase.New()

				m_Addresses = New System.Collections.Generic.List(Of System.Net.Mail.MailAddress)
			End Sub

			''' <summary>Parametered Constructor (1 Parameters)</summary>
			Public Sub New( _
				ByVal _Sending_Format As SendingType _
			)

				MyBase.New()

				Sending_Format = _Sending_Format

				m_Addresses = New System.Collections.Generic.List(Of System.Net.Mail.MailAddress)
			End Sub

			''' <summary>Parametered Constructor (2 Parameters)</summary>
			Public Sub New( _
				ByVal _Sending_Format As SendingType, _
				ByVal _Addresses As System.Collections.Generic.List(Of System.Net.Mail.MailAddress) _
			)

				MyBase.New()

				Sending_Format = _Sending_Format
				Addresses = _Addresses

			End Sub

		#End Region

		#Region " Public Constants "

			''' <summary>Public Shared Reference to the Name of the Property: Sending_Format</summary>
			''' <remarks></remarks>
			Public Const PROPERTY_SENDING_FORMAT As String = "Sending_Format"

			''' <summary>Public Shared Reference to the Name of the Property: Addresses</summary>
			''' <remarks></remarks>
			Public Const PROPERTY_ADDRESSES As String = "Addresses"

		#End Region

		#Region " Private Variables "

			''' <summary>Private Data Storage Variable for Property: Sending_Format</summary>
			''' <remarks></remarks>
			Private m_Sending_Format As SendingType

			''' <summary>Private Data Storage Variable for Property: Addresses</summary>
			''' <remarks></remarks>
			Private m_Addresses As System.Collections.Generic.List(Of System.Net.Mail.MailAddress)

		#End Region

		#Region " Public Properties "

			''' <summary>Provides Access to the Property: Sending_Format</summary>
			''' <remarks></remarks>
			Public Property Sending_Format() As SendingType
				Get
					Return m_Sending_Format
				End Get
				Set(value As SendingType)
					m_Sending_Format = value
				End Set
			End Property

			''' <summary>Provides Access to the Property: Addresses</summary>
			''' <remarks></remarks>
			Public Property Addresses() As System.Collections.Generic.List(Of System.Net.Mail.MailAddress)
				Get
					Return m_Addresses
				End Get
				Set(value As System.Collections.Generic.List(Of System.Net.Mail.MailAddress))
					m_Addresses = value
				End Set
			End Property

		#End Region

	End Class

End Namespace