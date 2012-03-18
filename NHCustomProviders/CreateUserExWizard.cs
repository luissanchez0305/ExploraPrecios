using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NHCustomProviders
{
	/// <summary>Extension of CreateUserWizard control to allow creating users with extra information.</summary>
	/// <remarks>As the CreateUserWizard control has some key methods and fields set as private we have to
	/// override some methods and do some dirty things to obtain the results we want. This is not the best 
	/// way to do what we want but it is the only one to avoid having to code a CreateUserWizard control
	/// from scratch due to the incredibly lack of extensibility of some ASP.NET controls.</remarks>
	public class CreateUserExWizard : CreateUserWizard
	{
		#region Fields

		private static readonly object _eventCreateUserEx = new object();

		#endregion

		#region Properties

		/// <summary>Gets or sets a value indicating whether CreateUserEx failed.</summary>
		protected bool CreateUserExFailed { get; set; }

		/// <summary>Gets or sets the error message.</summary>
		protected string ErrorMessage { get; set; }

		#endregion

		#region Events

		/// <summary>Event generated when an user with extra information can to be created.</summary>
		[Description("Event generated when an user with extra information can to be created."), Category("Action")]
		public event EventHandler<CreateUserExEventArgs> CreateUserEx
		{
			add { base.Events.AddHandler(_eventCreateUserEx, value); }
			remove { base.Events.RemoveHandler(_eventCreateUserEx, value); }
		}

		#endregion

		#region Methods

		#region Constructors

		/// <summary>Initializes a new instance of the CreateUserExWizard class.</summary>
		public CreateUserExWizard() : base()
		{
		}

		#endregion

		#region Method Overrides

		/// <summary>Raises the CreatedUseEx and CreatedUser events after the CreateUser method of the membership provider is called.</summary>
		/// <param name="e">An EventArgs that contains the event data.</param>
		protected override void OnCreatedUser(EventArgs e)
		{
			try {
				string encodedPassword = null;
				string encodedPasswordSalt = null;
				string encodedAnswer = null;

				// validates the user creation data and generates the encoded password and answer
				// this has to be done because there is no way to access to the data retured by the CreateUser
				// method because of the bad design of the CreateUserWizard control
				MembershipProvider provider = GetProvider(MembershipProvider);

				// generate the password if necessary
				string generatedPassword = null;

				if (AutoGeneratePassword) {
					int length = Math.Max(10, Membership.MinRequiredPasswordLength);
					generatedPassword = Membership.GeneratePassword(length, Membership.MinRequiredNonAlphanumericCharacters);
				}

				// validate the user creation data
				NHCustomMembershipProvider nhProvider = provider as NHCustomMembershipProvider;
				if (nhProvider != null) {
					MembershipCreateStatus status;

					string finalPassword = AutoGeneratePassword ? generatedPassword : ExtractCreateUserStepTextBoxData(Password, "Password");

					if (!nhProvider.ValidateUserCreation(
					     	ExtractCreateUserStepTextBoxData(UserName, "UserName"),
					     	finalPassword,
					     	ExtractCreateUserStepTextBoxData(Email, "Email"),
					     	ExtractCreateUserStepTextBoxData(Question, "Question"),
					     	ExtractCreateUserStepTextBoxData(Answer, "Answer"),
					     	!DisableCreatedUser,
					     	null,
					     	out status, out encodedPassword, out encodedPasswordSalt, out encodedAnswer)
						) {
						// throw an error with the appropiate message
						throw new MembershipCreateUserException(GetMembershipErrorMessage(provider, status));
					}
				}

				// raises the CreateUserEx event
				OnCreateUserEx(new CreateUserExEventArgs(encodedPassword, encodedPasswordSalt, encodedAnswer));
			} catch (Exception exc) {
				// if there is any error, rethrow the exception after generating the CreateUserError event
				OnCreateUserError(new CreateUserErrorEventArgs(MembershipCreateStatus.InvalidProviderUserKey));

				throw new MembershipCreateUserException("Error creating user", exc);
			}

			// raises the CreatedUser event
			base.OnCreatedUser(e);
		}

		/// <summary>Raises the NextButtonClick event when the user clicks the Next button in one of the Create User wizard steps.</summary>
		/// <param name="e">Data about the current navigation settings.</param>
		protected override void OnNextButtonClick(WizardNavigationEventArgs e)
		{
			try {
				// try the standard next behaviour
				base.OnNextButtonClick(e);
			} catch (MembershipCreateUserException exc) {
				// capture any exceptions thrown by the CreateUserEx event, showing the error and cancelling navigation
				if (WizardSteps[e.CurrentStepIndex] == CreateUserStep) {
					e.Cancel = true;

					CreateUserExFailed = true;
					ErrorMessage = exc.Message;
				}
			}
		}

		/// <summary>Renders the contents of the control to the specified writer. This method is used primarily by control developers.</summary>
		/// <param name="writer">A System.Web.UI.HtmlTextWriter that represents the output stream to render HTML content on the client.</param>
		protected override void RenderContents(HtmlTextWriter writer)
		{
			// adjust the error message properties based on the status of the CreateUserEx operation
			ApplyCreateUserExProperties();

			base.RenderContents(writer);
		}

		/// <summary>Gets the state of the design mode.</summary>
		/// <returns>The state of the design mode.</returns>
		protected override IDictionary GetDesignModeState()
		{
			IDictionary state = base.GetDesignModeState();

			// as the parent method can change the error label visibility, reset it here
			ApplyCreateUserExProperties();

			return state;
		}

		#endregion

		#region Helper Methods

		/// <summary>Raises the CreateUserEx event after the ValidateUserCreation method of the membership provider is called.</summary>
		/// <remarks>Here the user can insert their own users with the extra data they want. The 
		/// NHibernateCustomMembershipProvider has the ignoreCreateUserMethod attribute to avoid
		/// having the user created in the CreateUser method, so this can work, as the CreateUserWizard
		/// has near to no extension capabilities due to really bad design.</remarks>
		/// <param name="e">The CreateUserExEventArgs instance containing the event data.</param>
		protected virtual void OnCreateUserEx(CreateUserExEventArgs e)
		{
			EventHandler<CreateUserExEventArgs> handler = (EventHandler<CreateUserExEventArgs>)base.Events[_eventCreateUserEx];
			if (handler != null) {
				handler(this, e);
			}
		}

		/// <summary>Applies some changes to a few properties based on the status of the CreateUserEx operation.</summary>
		protected virtual void ApplyCreateUserExProperties()
		{
			// if there is an error, show it
			if (CreateUserExFailed && !String.IsNullOrEmpty(ErrorMessage)) {
				// find the control that contains the error message label
				Control control = CreateUserStep.ContentTemplateContainer.FindControl("ErrorMessage");
				ITextControl errorMessageLabel = control as ITextControl;

				// if the control has been found, set the error message and make it visible
				if (errorMessageLabel != null) {
					errorMessageLabel.Text = ErrorMessage;
					control.Visible = true;

					Control parent = control.Parent;
					if (parent != null) {
						parent.Visible = true;
					}

					WebControl webControl = control as WebControl;

					if (webControl != null) {
						webControl.ApplyStyle(ErrorMessageStyle);
					}
				}
			}
		}

		/// <summary>Gets an error message based on the status from the membership operation.</summary>
		/// <param name="provider">The provider.</param>
		/// <param name="status">The status of the membership operation.</param>
		/// <returns>A descriptive error message.</returns>
		protected virtual string GetMembershipErrorMessage(MembershipProvider provider, MembershipCreateStatus status)
		{
			string errorMessage;

			switch (status) {
				case MembershipCreateStatus.InvalidPassword: {
					string invalidPasswordErrorMessage = InvalidPasswordErrorMessage;
					if (!String.IsNullOrEmpty(invalidPasswordErrorMessage)) {
						invalidPasswordErrorMessage = string.Format(CultureInfo.InvariantCulture, invalidPasswordErrorMessage, new object[] { provider.MinRequiredPasswordLength, provider.MinRequiredNonAlphanumericCharacters });
					}
					errorMessage = invalidPasswordErrorMessage;
					break;
				}
				case MembershipCreateStatus.InvalidQuestion:
					errorMessage = InvalidQuestionErrorMessage;
					break;

				case MembershipCreateStatus.InvalidAnswer:
					errorMessage = InvalidAnswerErrorMessage;
					break;

				case MembershipCreateStatus.InvalidEmail:
					errorMessage = InvalidEmailErrorMessage;
					break;

				case MembershipCreateStatus.DuplicateUserName:
					errorMessage = DuplicateUserNameErrorMessage;
					break;

				case MembershipCreateStatus.DuplicateEmail:
					errorMessage = DuplicateEmailErrorMessage;
					break;

				default:
					errorMessage = UnknownErrorMessage;
					break;
			}

			return errorMessage;
		}

		/// <summary>Gets the provider used by the control.</summary>
		/// <param name="providerName">Name of the provider to retrieve.</param>
		/// <returns>The requested provider, or the default provider if no provider was specified.</returns>
		protected static MembershipProvider GetProvider(string providerName)
		{
			// if no provider specified, return the default provider
			if (String.IsNullOrEmpty(providerName)) {
				return Membership.Provider;
			}

			// try get the requested provider. If not found, error
			MembershipProvider provider = Membership.Providers[providerName];

			if (provider == null) {
				throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, "Error, cannot find the membership provider called {0}", providerName), "providerName");
			}

			return provider;
		}

		/// <summary>Extracts data from the create user wizard step.</summary>
		/// <param name="value">The value stored.</param>
		/// <param name="controlName">Name of the control to retrieve the value.</param>
		/// <returns>The requested value.</returns>
		protected virtual string ExtractCreateUserStepTextBoxData(string value, string controlName)
		{
			string result = value;

			if (String.IsNullOrEmpty(result) && (CreateUserStep.ContentTemplateContainer != null)) {
				Control control = CreateUserStep.ContentTemplateContainer.FindControl(controlName);
				ITextControl textBox = control as ITextControl;

				if (textBox != null) {
					result = textBox.Text;
				}
			}

			return result;
		}

		#endregion

		#endregion
	}
}