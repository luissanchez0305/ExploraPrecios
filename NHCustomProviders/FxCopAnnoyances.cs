//===============================================================================
// This file contains some warning suppressions for FxCop 1.36 that aren't 
// placed in the associated items to avoid polluting the code with attributes. 
// Hopefully some day, the FxCop developers will realize that suppressing the 
// same warnings with a single line is something really useful instead of having 
// us to add a suppression message for some rules that are plain stupid. 
//===============================================================================

using System;
using System.Diagnostics.CodeAnalysis;

// suppress warnings for using output parameters

[assembly: SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", Scope = "member", Target = "NHCustomProviders.MembershipUserInfoDal.#GetAll(System.Int32,System.Int32,System.Int32&)", MessageId = "2#")]
[assembly: SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", Scope = "member", Target = "NHCustomProviders.MembershipUserInfoDal.#GetByEmail(System.String,System.Int32,System.Int32,System.Int32&)", MessageId = "3#")]
[assembly: SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", Scope = "member", Target = "NHCustomProviders.MembershipUserInfoDal.#GetByUserName(System.String,System.Int32,System.Int32,System.Int32&)", MessageId = "3#")]
[assembly: SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", Scope = "member", Target = "NHCustomProviders.NHCustomMembershipProvider.#GetEncodedStringByUserSettings(NHibernate.ISession,System.String,System.String,NHCustomProviders.MembershipUserInfo&)", MessageId = "3#")]
[assembly: SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", Scope = "member", Target = "NHCustomProviders.NHCustomMembershipProvider.#ValidatePassword(System.String,System.String,System.Web.Security.ValidatePasswordEventArgs&)", MessageId = "2#")]
[assembly: SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", Scope = "member", Target = "NHCustomProviders.NHCustomMembershipProvider.#ValidateUserCreation(System.String,System.String,System.String,System.String,System.String,System.Boolean,System.Object,System.Web.Security.MembershipCreateStatus&,System.String&,System.String&,System.String&)", MessageId = "10#")]
[assembly: SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", Scope = "member", Target = "NHCustomProviders.NHCustomMembershipProvider.#ValidateUserCreation(System.String,System.String,System.String,System.String,System.String,System.Boolean,System.Object,System.Web.Security.MembershipCreateStatus&,System.String&,System.String&,System.String&)", MessageId = "8#")]
[assembly: SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", Scope = "member", Target = "NHCustomProviders.NHCustomMembershipProvider.#ValidateUserCreation(System.String,System.String,System.String,System.String,System.String,System.Boolean,System.Object,System.Web.Security.MembershipCreateStatus&,System.String&,System.String&,System.String&)", MessageId = "9#")]
[assembly: SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", Scope = "member", Target = "NHCustomProviders.NHCustomMembershipProvider.#ValidateUserCreation(System.String,System.String,System.String,System.String,System.String,System.Boolean,System.Object,System.Web.Security.MembershipCreateStatus&,System.String&,System.String&,System.String&)", MessageId = "7#")]
[assembly: SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", Scope = "member", Target = "NHCustomProviders.NHCustomRoleProvider.#ConfigureNHibernate(NHibernate.Cfg.Configuration&,NHibernate.Cfg.Configuration&)", MessageId = "0#")]
[assembly: SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", Scope = "member", Target = "NHCustomProviders.NHCustomRoleProvider.#ConfigureNHibernate(NHibernate.Cfg.Configuration&,NHibernate.Cfg.Configuration&)", MessageId = "1#")]
[assembly: SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", Scope = "member", Target = "NHCustomProviders.XmlDocHelper.#CreateClassMappingDocument(System.String,System.String,System.Xml.XmlElement&)", MessageId = "2#")]