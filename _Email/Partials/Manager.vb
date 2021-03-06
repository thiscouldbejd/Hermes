Imports System
Imports System.Diagnostics
Imports System.Net
Imports System.Security
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Text.RegularExpressions.Regex

Namespace Email

	Public Partial Class Manager

		#Region " Private Constants "

			''' <summary>
			''' Regular Expression Validation String
			''' </summary>
			Private Const EMAIL_VALIDATION_REGEX As String = _
				"^[-a-z0-9~!$%^&*_=+}{\'?]+(\.[-a-z0-9~!$%^&*_=+}{\'?]+)*@([a-z0-9_][-a-z0-9_]*(\.[-a-z0-9_]+)*\.(aero|arpa|biz|com|coop|edu|gov|info|int|mil|museum|name|net|org|pro|travel|mobi|[a-z][a-z])|([0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}))(:[0-9]{1,5})?$" ' "^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.(?:[A-Z]{2}.[A-Z]{2}|[A-Z]{2}|com|org|net|edu|gov|mil|biz|info|mobi|name|aero|asia|jobs|museum)$"

			' -- Attributes for Log --
			Private Const ATTRIBUTE_DATETIMESTAMP As String = "dt-stamp"

			Private Const ATTRIBUTE_TYPE As String = "type"

			Private Const ATTRIBUTE_ENCODING As String = "encoding"

			Private Const ATTRIBUTE_DISPOSITION As String = "dispostion"

			Private Const ATTRIBUTE_NAME As String = "name"

			Private Const ATTRIBUTE_ID As String = "id"

			Private Const ATTRIBUTE_URI As String = "uri"

			Private Const ATTRIBUTE_AUTOGENERATED As String = "auto-generated"

			Private Const ATTRIBUTE_LOCATION As String = "location"

			Private Const ATTRIBUTE_FROM As String = "from"

			Private Const ATTRIBUTE_TO As String = "to"


			' -- Document Element for Log --
			Private Const ELEMENT_LOG As String = "Log"


			' -- General Details for Log --
			Private Const ELEMENT_APPLICATION As String = "Application"

			Private Const ELEMENT_JOB As String = "Job"

			Private Const ELEMENT_USER As String = "User"


			' -- Message Details for Log --
			Private Const ELEMENT_MESSAGE As String = "Message"

			Private Const ELEMENT_SUBJECT As String = "Subject"

			Private Const ELEMENT_BODY As String = "Body"

			Private Const ELEMENT_VIEWS As String = "Views"

			Private Const ELEMENT_VIEW As String = "View"

			Private Const ELEMENT_RESOURCES As String = "Resources"

			Private Const ELEMENT_RESOURCE As String = "Resource"

			Private Const ELEMENT_ATTACHMENTS As String = "Attachments"

			Private Const ELEMENT_ATTACHMENT As String = "Attachment"

			Private Const ELEMENT_CONTENT As String = "Content"

			Private Const ELEMENT_HEADERS As String = "Headers"

			Private Const ELEMENT_HEADER As String = "Header"

			Private Const ELEMENT_REPLACEMENTS As String ="Replacements"

			Private Const ELEMENT_REPLACEMENT As String ="Replaced"



			' -- Transport Details for Log --
			Private Const ELEMENT_TRANSPORT As String = "Transport"

			Private Const ELEMENT_SERVER As String = "Server"

			Private Const ELEMENT_PORT As String = "Port"


			' -- Delivery Details for Log --
			Private Const ELEMENT_DELIVERY As String = "Delivery"

			Private Const ELEMENT_BATCH As String = "Batch"

			Private Const ELEMENT_FROM As String = "From"

			Private Const ELEMENT_TO As String = "To"

			Private Const ELEMENT_BCC As String = "Blind-Carbon-Copy"

			Private Const ELEMENT_CC As String = "Carbon-Copy"

			Private Const ELEMENT_REPLYTO As String = "Reply-To"

			Private Const ELEMENT_ADDRESS As String = "Address"

			Private Const ELEMENT_RESULT As String = "Result"

			Private Const ELEMENT_STATUS As String = "Status"

			Private Const ELEMENT_DETAILS As String = "Details"

			Private Const ELEMENT_LINE As String = "Line"


			Private Const REPLACE_ADDRESS = "%7bHERMES-ADDRESS%7d"

		#End Region

		#Region " Private Properties "

			Private ReadOnly Property Daily_SubDirectoryName As String
				Get
					Return Logging_Day.ToString("yyyyMMdd")
				End Get
			End Property

			Private ReadOnly Property Logging_Enabled As Boolean
				Get

					Dim return_Value As Boolean = True

					' Also should check that we can write a log
					If Logging_Directory Is Nothing Then

						Debug.WriteLine("HERMES: No Logging Directory Configured")
						return_Value = False

					ElseIf Not Logging_Directory.Exists Then

						Debug.WriteLine(String.Format("HERMES: Logging Directory [{0}] Doesn't Exist", Logging_Directory.FullName))
						return_Value = False

					Else

						If Not IO.Directory.Exists(IO.Path.Combine(Logging_Directory.FullName, Daily_SubDirectoryName)) Then

							Try

								IO.Directory.CreateDirectory(IO.Path.Combine(Logging_Directory.FullName, Daily_SubDirectoryName))

							Catch ex As Exception

								TraceException(ex)
								return_Value = False

							End Try

						End If

					End If

					Debug.WriteLine("HERMES: Logging Enabled = " & return_Value.ToString())

					Return return_Value

				End Get
			End Property

			Private ReadOnly Property SMTP_Server_Address As String
				Get
					Return SMTP_Server.AddressList(0).ToString
				End Get
			End Property

		#End Region

		#Region " Private Methods "

			Private Function GenerateCredential() As ICredentialsByHost

				If Not SMTP_Credential Is Nothing Then

					Return SMTP_Credential

				Else

					Return Nothing

				End If

			End Function

			Private Function GenerateLog() As System.Xml.XmlWriter

				Dim job_Identifier As Guid = Guid.NewGuid()

				Dim return_Log As New System.Xml.XmlTextWriter(IO.Path.Combine(Logging_Directory.FullName, _
					Daily_SubDirectoryName, String.Format("{0}.xml", job_Identifier.ToString("D"))), System.Text.Encoding.UTF8)

				return_Log.Formatting = System.Xml.Formatting.Indented
				return_Log.IndentChar = Microsoft.VisualBasic.ChrW(&H9)
				return_Log.WriteStartDocument()

				return_Log.WriteStartElement(ELEMENT_LOG)

				return_Log.WriteAttributeString(ATTRIBUTE_DATETIMESTAMP, _
					System.Xml.XmlConvert.ToString(DateTime.Now(), System.Xml.XmlDateTimeSerializationMode.Local))

				Return return_Log

			End Function

			Private Sub LogMailAddressCollection( _
				ByRef log As System.Xml.XmlWriter, _
				ByVal element_Name As String, _
				ByVal address_Collection As System.Collections.ObjectModel.Collection(Of Mail.MailAddress) _
			)

				If Not address_Collection Is Nothing AndAlso address_Collection.Count > 0 Then

					log.WriteStartElement(element_Name)

					For i As Integer = 0 To address_Collection.Count - 1

						log.WriteStartElement(ELEMENT_ADDRESS)
						log.WriteAttributeString(ATTRIBUTE_TYPE, "smtp")
						log.WriteString(address_Collection(i).ToString())
						log.WriteEndElement()

					Next

					log.WriteEndElement()

				End If

			End Sub

			Private Sub LogAttachmentBase( _
				ByRef log As System.Xml.XmlWriter, _
				ByVal value As Mail.AttachmentBase _
			)

				If Not String.IsNullOrEmpty(value.ContentId) Then _
					log.WriteAttributeString(ATTRIBUTE_ID, value.ContentId)

				log.WriteAttributeString(ATTRIBUTE_TYPE, value.ContentType.ToString())

				log.WriteAttributeString(ATTRIBUTE_ENCODING, value.TransferEncoding.ToString())

				' -- Log Contents Here if Required --
				If value.ContentType.ToString.StartsWith("text/", System.StringComparison.OrdinalIgnoreCase) Then

					Dim start_Position As Int64 = value.ContentStream.Position
					log.WriteStartElement(ELEMENT_CONTENT)

					If value.TransferEncoding = System.Net.Mime.TransferEncoding.QuotedPrintable Then

						Dim ascii_Reader As New IO.StreamReader(value.ContentStream, System.Text.Encoding.ASCII)
						log.WriteCData(ascii_Reader.ReadToEnd())

					ElseIf value.TransferEncoding = System.Net.Mime.TransferEncoding.Base64 Then

						Dim utf8_Reader As New IO.StreamReader(value.ContentStream, System.Text.Encoding.UTF8)
						log.WriteCData(utf8_Reader.ReadToEnd())

					End If

					log.WriteEndElement()
					value.ContentStream.Position = start_Position

				End If

			End Sub

			Private Sub LogException( _
				ByRef log As System.Xml.XmlWriter, _
				ByVal element_Name As String, _
				ByVal ex As Exception _
			)

				Dim exception_String As String = ex.ToString()

				If Not String.IsNullOrEmpty(exception_String) Then

					Dim exception_Strings As String() = exception_String.Split( _
						Microsoft.VisualBasic.ChrW(&HA), Microsoft.VisualBasic.ChrW(&HD))

					If exception_Strings.Length = 1 Then

						log.WriteElementString(element_Name, exception_Strings(0))

					ElseIf exception_Strings.Length > 1 Then

						log.WriteStartElement(element_Name)

						For i As Integer = 0 To exception_Strings.Length - 1

							If Not String.IsNullOrEmpty(exception_Strings(i)) Then _
								log.WriteElementString(ELEMENT_LINE, exception_Strings(i))

						Next

						log.WriteEndElement()

					End If

				End If

			End Sub

			Private Sub TraceException( _
				ByVal ex As Exception _
			)

				Dim exception_String As String = ex.ToString()

				If Not String.IsNullOrEmpty(exception_String) Then

					Dim exception_Strings As String() = exception_String.Split( _
						Microsoft.VisualBasic.ChrW(&HA), Microsoft.VisualBasic.ChrW(&HD))

					If exception_Strings.Length = 1 Then

						Debug.WriteLine(exception_Strings(0))

					ElseIf exception_Strings.Length > 1 Then

						For i As Integer = 0 To exception_Strings.Length - 1

							If Not String.IsNullOrEmpty(exception_Strings(i)) Then _
								Debug.WriteLine(exception_Strings(i))

						Next

					End If

				End If

			End Sub

			Public Function ValidateCertificate( _
				ByVal sender As Object, _
				ByVal certification As System.Security.Cryptography.X509Certificates.X509Certificate, _
				ByVal chain As System.Security.Cryptography.X509Certificates.X509Chain, _
				ByVal sslPolicyErrors As System.Net.Security.SslPolicyErrors _
			) As Boolean
        
				Return True

			End Function

		#End Region

		#Region " Public Methods "

			Public Function SendMessage( _
				ByVal sender As Mail.MailAddress, _
				ByVal recipient As Distribution, _
				ByVal message As Message, _
				Optional ByVal summary_To As Mail.MailAddress = Nothing, _
				Optional ByVal job_Details As String = Nothing, _
				Optional ByVal reply_To As Mail.MailAddress = Nothing _
			) As Boolean

				Dim return_Result As Boolean = True

				Dim log As System.Xml.XmlWriter = Nothing
				If Logging_Enabled Then log = GenerateLog()

				If Not log Is Nothing Then

					' -- Write General Log Details --
					If Not String.IsNullOrEmpty(Client_Application) Then _
						log.WriteElementString(ELEMENT_APPLICATION, Client_Application)

					If Not System.Threading.Thread.CurrentPrincipal Is Nothing _
						AndAlso Not System.Threading.Thread.CurrentPrincipal.Identity Is Nothing AndAlso _
						System.Threading.Thread.CurrentPrincipal.Identity.IsAuthenticated Then _
						log.WriteElementString(ELEMENT_USER, System.Threading.Thread.CurrentPrincipal.Identity.Name)

					If Not String.IsNullOrEmpty(job_Details) Then _
						log.WriteElementString(ELEMENT_JOB, job_Details)

					' -- Write Transport Log Details --
					log.WriteStartElement(ELEMENT_TRANSPORT)
					log.WriteAttributeString(ATTRIBUTE_TYPE, "smtp")

					log.WriteElementString(ELEMENT_SERVER, SMTP_Server.HostName)
					log.WriteElementString(ELEMENT_PORT, SMTP_Port.ToString())
					If Not SMTP_Credential Is Nothing Then
						If Not String.IsNullOrEmpty(SMTP_Credential.Domain) AndAlso _
							Not String.IsNullOrEmpty(SMTP_Credential.Username) Then
							log.WriteElementString(ELEMENT_USER, String.Format("{0}\{1}", SMTP_Credential.Domain, SMTP_Credential.Username))
						Else
								log.WriteElementString(ELEMENT_USER, "Thread-Delegated")
						End If
					End If

					log.WriteEndElement()

				End If

				Dim email_To_Send As Mail.MailMessage = message.ToMailMessage()
				If Not String.IsNullOrEmpty(Client_Application) Then _
					email_To_Send.Headers.Add(Message.SENDER, Client_Application)

				If Not log Is Nothing Then

					' -- Write Message Log Details --
					log.WriteStartElement(ELEMENT_MESSAGE)
					If message.AutoGenerated Then log.WriteAttributeString(ATTRIBUTE_AUTOGENERATED, "true")

					' Subject
					log.WriteElementString(ELEMENT_SUBJECT, email_To_Send.Subject)

					' Body
					If Not String.IsNullOrEmpty(email_To_Send.Body) Then

						log.WriteStartElement(ELEMENT_BODY)
						log.WriteAttributeString(ATTRIBUTE_ENCODING, email_To_Send.BodyEncoding.ToString())
						If email_To_Send.IsBodyHtml Then
							log.WriteAttributeString(ATTRIBUTE_TYPE, "html")
						Else
							log.WriteAttributeString(ATTRIBUTE_TYPE, "text")
						End If
						log.WriteCData(email_To_Send.Body)
						log.WriteEndElement()

					End If

					If Not email_To_Send.AlternateViews Is Nothing AndAlso email_To_Send.AlternateViews.Count > 0 Then

						log.WriteStartElement(ELEMENT_VIEWS)

						For i As Integer = 0 To email_To_Send.AlternateViews.Count - 1

							log.WriteStartElement(ELEMENT_VIEW)

							LogAttachmentBase(log, email_To_Send.AlternateViews(i))

							If Not email_To_Send.AlternateViews(i).LinkedResources Is Nothing AndAlso _
								email_To_Send.AlternateViews(i).LinkedResources.Count > 0 Then

								log.WriteStartElement(ELEMENT_RESOURCES)

								For j As Integer = 0 To email_To_Send.AlternateViews(i).LinkedResources.Count - 1

									log.WriteStartElement(ELEMENT_RESOURCE)

									If Not email_To_Send.AlternateViews(i).LinkedResources(j).ContentLink Is Nothing Then _
										log.WriteAttributeString(ATTRIBUTE_URI, email_To_Send.AlternateViews(i). _
										LinkedResources(j).ContentLink.ToString())
									LogAttachmentBase(log, email_To_Send.AlternateViews(i).LinkedResources(j))

									log.WriteEndElement()

								Next

								log.WriteEndElement()

							End If

							log.WriteEndElement()

						Next

						log.WriteEndElement()

					End If

					' Attachments
					If Not email_To_Send.Attachments Is Nothing AndAlso email_To_Send.Attachments.Count > 0 Then

						log.WriteStartElement(ELEMENT_ATTACHMENTS)

						For i As Integer = 0 To email_To_Send.Attachments.Count - 1

							log.WriteStartElement(ELEMENT_ATTACHMENT)

							log.WriteAttributeString(ATTRIBUTE_DISPOSITION, email_To_Send.Attachments(i).ContentDisposition.ToString())
							If Not String.IsNullOrEmpty(email_To_Send.Attachments(i).Name) Then log. _
								WriteAttributeString(ATTRIBUTE_NAME, email_To_Send.Attachments(i).Name)

							LogAttachmentBase(log, email_To_Send.Attachments(i))

							log.WriteEndElement()

						Next

						log.WriteEndElement()

					End If

					log.WriteEndElement()

				End If

				' -- Write Delivery Log Details --
				If Not log Is Nothing Then log.WriteStartElement(ELEMENT_DELIVERY)

				email_To_Send.From = sender
				If Not reply_To Is Nothing Then email_To_Send.ReplyToList.Add(reply_To)

				Dim recipients As Mail.MailAddress()() = recipient.Split(SMTP_MaxBatch)

				Dim orginal_Body As String = email_To_Send.Body

				For i As Integer = 0 To recipients.Length - 1

					' Clear All Current Recipients
					email_To_Send.Bcc.Clear()
					email_To_Send.Cc.Clear()
					email_To_Send.To.Clear()

					If recipients(i).Length = 1 Then

						email_To_Send.To.Add(recipients(i)(0))

						Dim index_At = orginal_Body.IndexOf(REPLACE_ADDRESS)

						If index_At >= 0 Then

							email_To_Send.Body = orginal_Body.Replace(REPLACE_ADDRESS, recipients(i)(0).Address)

							If Not log Is Nothing Then

								log.WriteStartElement(ELEMENT_REPLACEMENTS)
								log.WriteStartElement(ELEMENT_REPLACEMENT)
								log.WriteAttributeString(ATTRIBUTE_LOCATION, CStr(index_At))
								log.WriteAttributeString(ATTRIBUTE_FROM, "hermes-address")
								log.WriteAttributeString(ATTRIBUTE_TO, recipients(i)(0).Address)
								log.WriteEndElement()
								log.WriteEndElement()

							End If

						ElseIf Not log Is Nothing Then

							log.WriteStartElement(ELEMENT_REPLACEMENTS)
							log.WriteEndElement()

						End If

					ElseIf recipients(i).Length > 1 Then

						email_To_Send.To.Add(sender)

						For j As Integer = 0 To recipients(i).Length - 1

							email_To_Send.Bcc.Add(recipients(i)(j))

						Next

					End If

					If Not log Is Nothing Then

						' -- Write Batch Log Details --
						log.WriteStartElement(ELEMENT_BATCH)
						log.WriteAttributeString(ATTRIBUTE_DATETIMESTAMP, _
							System.Xml.XmlConvert.ToString(DateTime.Now(), System.Xml.XmlDateTimeSerializationMode.Local))

						' -- Write From --
						Dim from_Collection As New Mail.MailAddressCollection
						from_Collection.Add(email_To_Send.From)
						LogMailAddressCollection(log, ELEMENT_FROM, from_Collection)

						If Not email_To_Send.ReplyToList Is Nothing AndAlso email_To_Send.ReplyToList.Count > 0 Then
							' -- Write Reply To --
							LogMailAddressCollection(log, ELEMENT_REPLYTO, email_To_Send.ReplyToList)
						End If

						' -- Write To --
						LogMailAddressCollection(log, ELEMENT_TO, email_To_Send.To)

						' -- Write CC --
						LogMailAddressCollection(log, ELEMENT_CC, email_To_Send.CC)

						' -- Write BCC --
						LogMailAddressCollection(log, ELEMENT_BCC, email_To_Send.BCC)

					End If

					Dim email_Transport As Mail.SmtpClient

					Try

						' Stops Trying to Send Messages in Case of Error or Send Suppression
						If Not Suppress_Send AndAlso return_Result Then

							email_Transport = New Mail.SmtpClient(SMTP_Server_Address, SMTP_Port)
							email_Transport.EnableSsl = Use_SSL

							If Use_SSL AndAlso Not Verify_SSL Then _
								ServicePointManager.ServerCertificateValidationCallback = AddressOf ValidateCertificate

							email_Transport.Timeout = (SMTP_Timeout * 1000)
							email_Transport.Credentials = GenerateCredential()
							If email_Transport.Credentials Is Nothing Then email_Transport.UseDefaultCredentials = True
							email_Transport.Send(email_To_Send)

							If Not log Is Nothing Then log.WriteElementString(ELEMENT_RESULT, "Success")

						Else

							If Not log Is Nothing Then log.WriteElementString(ELEMENT_RESULT, "Not-Attempted")

						End If

					Catch recipient_Ex As Mail.SmtpFailedRecipientsException

						If Not log Is Nothing Then

							log.WriteStartElement(ELEMENT_RESULT)
							log.WriteElementString(ELEMENT_STATUS, "Recipient-Failed")
							If Not recipient_Ex.FailedRecipient Is Nothing Then _
								log.WriteElementString(ELEMENT_DETAILS, recipient_Ex.FailedRecipient.ToString())

							log.WriteEndElement()

						End If

					Catch smtp_Ex As Mail.SmtpException

						return_Result = False

						If Not log Is Nothing Then

							log.WriteStartElement(ELEMENT_RESULT)

							log.WriteElementString(ELEMENT_STATUS, smtp_Ex.StatusCode.ToString())
							LogException(log, ELEMENT_DETAILS, smtp_Ex)
							If Not smtp_Ex.InnerException Is Nothing Then _
								LogException(log, ELEMENT_DETAILS, smtp_Ex.InnerException)

							log.WriteEndElement()

						End If

					Catch ex As Exception

						return_Result = False

						If Not log Is Nothing Then

							log.WriteStartElement(ELEMENT_RESULT)
							log.WriteElementString(ELEMENT_STATUS, "Failed")
							LogException(log, ELEMENT_DETAILS, ex)

							log.WriteEndElement()

						End If

					Finally

						' Writes the End of the Batch
						If Not log Is Nothing Then log.WriteEndElement()

					End Try

				Next

				' -- End Delivery Log Details --
				If Not log Is Nothing Then

					' Writes the End of the Delivery
					log.WriteEndElement()

					' Writes the Headers Detail (Populated After Send)
					If Not email_To_Send.Headers Is Nothing AndAlso email_To_Send.Headers.Count > 0 Then

						log.WriteStartElement(ELEMENT_HEADERS)
						If Not email_To_Send.HeadersEncoding Is Nothing Then _
							log.WriteAttributeString(ATTRIBUTE_ENCODING, email_To_Send.HeadersEncoding.ToString())

						For Each header_Name As String In email_To_Send.Headers.Keys

							log.WriteStartElement(ELEMENT_HEADER)
							log.WriteAttributeString(ATTRIBUTE_NAME, header_Name)
							log.WriteString(email_To_Send.Headers(header_Name))
							log.WriteEndElement()

						Next

						log.WriteEndElement()

					End If

					' Writes the End of the Job
					log.WriteEndElement()

					' Writes the End of the Document
					log.WriteEndDocument()

					log.Flush()
					log.Close()

				End If

				Return return_Result

			End Function

			Public Overrides Function ToString() As String

				Return Nothing

			End Function

		#End Region

		#Region " Private Shared Methods "

			''' <summary>
			''' Method to Validate a given String against a Regular Expression.
			''' </summary>
			''' <param name="stringToBeValidated">The String to Validate.</param>
			''' <param name="validationRegex">The Regex to Validate Against.</param>
			''' <returns>A Boolean value indicating whether the string is valid.</returns>
			''' <remarks>Will return True if the Regular Expression is Empty.</remarks>
			Private Shared Function ValidatesAgainstRegularExpression( _
				ByVal stringToBeValidated As String, _
				ByVal validationRegex As String _
			) As Boolean

				If String.IsNullOrEmpty(validationRegex) Then

					Return True

				Else

					If stringToBeValidated = Nothing Then stringToBeValidated = String.Empty

					Return IsMatch(stringToBeValidated, validationRegex, _
						RegexOptions.IgnorePatternWhitespace)

				End If

			End Function

		#End Region

		#Region " Public Shared Methods "

			Public Shared Function CreateAddress( _
				ByVal address As String, _
				Optional ByVal display As String = Nothing _
			) As Mail.MailAddress

				If Not String.IsNullOrEmpty(address) AndAlso ValidatesAgainstRegularExpression(address, EMAIL_VALIDATION_REGEX) Then

					Return New Mail.MailAddress(address, display)

				Else

					Debug.WriteLine(String.Format("Invalid Email Address: {0}", address))
					Throw New ArgumentException("Email Address must be present and Valid", "address")

				End If

			End Function

			Public Shared Function CreateServer( _
				ByVal server As String _
			) As IPHostEntry

				If Not String.IsNullOrEmpty(server) Then

					Dim server_HostEntry As IPHostEntry = Nothing

					Try

						server_HostEntry = Dns.GetHostEntry(server)

					Catch ex As Exception

						Throw New ArgumentException("Server Name/Address must be valid", "server", ex)

					End Try

					If Not server_HostEntry Is Nothing Then

						Return server_HostEntry

					Else

						Throw New ArgumentException("Server Name/Address must be valid", "server")

					End If

				Else

					Throw New ArgumentException("Server Name/Address must be present", "server")

				End If

			End Function

			Public Shared Function CreateIntegratedCredential() As NetworkCredential

				Return CredentialCache.DefaultNetworkCredentials

			End Function

			Public Shared Function CreateCredential( _
				byVal username As String, _
				ByVal password As String, _
				ByVal domain As String _
			) As NetworkCredential

				Dim secure_Password As New SecureString

				If Not String.IsNullOrEmpty(password) Then

					For i As Integer = 0 To password.Length - 1
						secure_Password.AppendChar(password(i))
					Next

				End If

				Return Manager.CreateCredential(username, secure_Password, domain)

			End Function

			Public Shared Function CreateCredential( _
				byVal username As String, _
				ByVal password As SecureString, _
				ByVal domain As String _
			) As NetworkCredential

				If String.IsNullOrEmpty(username) Then

					Throw New ArgumentException("Username must be present", "username")

				ElseIf Not username.IndexOf("@") >= 0 AndAlso String.IsNullOrEmpty(domain) Then

					Throw New ArgumentException("Domain must be present", "domain")

				Else

					Return New NetworkCredential(username, password, domain)

				End If

			End Function

			Public Shared Function CreateMessage( _
				ByVal subject As String _
			) As Message

				Return Manager.CreateMessage(BodyType.Text, Encoding.UTF8, subject)

			End Function

			Public Shared Function CreateMessage( _
				ByVal format As BodyType, _
				ByVal encoder As Encoding, _
				ByVal subject As String _
			) As Message

				If String.IsNullOrEmpty(subject) Then

					Throw New ArgumentException("Subject must be present", "subject")

				Else

					Return New Message(format, encoder, subject)

				End If

			End Function

		#End Region

		#Region " Private Shared Log Methods "

			Private Shared Sub LoadAddresses( _
				ByRef log_File As System.Xml.XmlReader, _
				ByRef addresses As List(Of String), _
				ByVal element_Name As String _
			)

				While log_File.Read

					If log_File.Name = element_Name AndAlso log_File.NodeType = Xml.XmlNodeType.EndElement Then

						Exit While

					ElseIf log_File.Name = ELEMENT_ADDRESS AndAlso log_File.NodeType = Xml.XmlNodeType.Element Then
						
						If Not log_File.IsEmptyElement AndAlso log_File.Read() AndAlso Not String.IsNullOrEmpty(log_File.Value) Then

							Dim start_Pos As Integer = log_File.Value.IndexOf("&lt;")
							Dim end_Pos As Integer = log_File.Value.IndexOf("&gt;")

							If start_Pos >= 0 AndAlso end_Pos >= 0 Then

								addresses.Add(log_File.Value.SubString(start_Pos, end_Pos - (start_Pos + 4)))
								
							Else

								addresses.Add(log_File.Value)

							End If

						End If

					End If

				End While

			End Sub

		#End Region

		#Region " Public Shared Log Methods "

			Public Shared Function LoadLogFile( _
				ByVal log_File As System.Xml.XmlReader, _
				ByRef successfull_Sends As IList, _
				ByVal failed_Sends As IList _
			) As Boolean

				Dim addresses As New List(Of String)

				While log_File.Read()

					If log_File.Name = ELEMENT_BATCH AndAlso log_File.NodeType = Xml.XmlNodeType.Element Then

						While log_File.Read

							If log_File.Name = ELEMENT_BATCH AndAlso log_File.NodeType = Xml.XmlNodeType.EndElement Then

								Exit While

							ElseIf log_File.Name = ELEMENT_CC AndAlso log_File.NodeType = Xml.XmlNodeType.Element Then

								LoadAddresses(log_File, addresses, ELEMENT_CC)

							ElseIf log_File.Name = ELEMENT_BCC AndAlso log_File.NodeType = Xml.XmlNodeType.Element Then

								LoadAddresses(log_File, addresses, ELEMENT_BCC)

							ElseIf log_File.Name = ELEMENT_TO AndAlso log_File.NodeType = Xml.XmlNodeType.Element Then

								LoadAddresses(log_File, addresses, ELEMENT_TO)

							ElseIf log_File.Name = ELEMENT_RESULT AndAlso log_File.NodeType = Xml.XmlNodeType.Element Then

								If Not log_File.IsEmptyElement AndAlso log_File.Read() AndAlso Not String.IsNullOrEmpty(log_File.Value) Then

									If log_File.Value = "Success" Then

										For Each address As String In addresses

											successfull_Sends.Add(address)

										Next

									Else

										For Each address As String In addresses

											failed_Sends.Add(address)

										Next

									End If

									addresses.Clear()

								End If

							End If

						End While

					ElseIf log_File.Name = ELEMENT_LOG AndAlso log_File.NodeType = Xml.XmlNodeType.EndElement Then

						Return True

					End If

				End While

				Return False

			End Function

		#End Region

	End Class

End Namespace