using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Explora_Precios.Web.Controllers.ViewModels
{
	public class VideoRegisterModel
	{
		public VideoRegisterModel()
		{

		}

		public string video_email { get; set; }
	}

	public class UserViewModel
	{
		public UserViewModel()
		{
			Gender = 'f';
			Birthdate = new DateTime(1, 1, 1);
		}

		public string Name { get; set; }
		public string LastName { get; set; }
		public DateTime Birthdate { get; set; }
		public char Gender { get; set; }
		public string Email { get; set; }
		public bool IsNew { get; set; }
		public string Password { get; set; }
		public string NewPassword { get; set; }
		public string ConfirmPassword { get; set; }
		public string Redirect { get; set; }
	}

	public class LoginViewModel
	{
		public LoginViewModel() { }

		public string Email { get; set; }
		public string Password { get; set; }
		public bool Remember { get; set; }
		public string Redirect { get; set; }

	}
}
