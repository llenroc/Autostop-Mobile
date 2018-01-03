﻿using System;
using System.Threading.Tasks;
using Autostop.Common.Shared.Models;
using Firebase.Auth;

namespace Autostop.Client.iOS
{
	class PhoneAuthenticationProvider
	{
		public Task<VerifyNumberResult> VerifyPhoneNumber(string phoneNumber)
		{
			var tcs = new TaskCompletionSource<VerifyNumberResult>();
			PhoneAuthProvider.DefaultInstance.VerifyPhoneNumber(phoneNumber, null, (id, error) =>
			{
				if (error != null)
				{
					tcs.SetException(new Exception(error.LocalizedDescription));
				}
				else
				{
					tcs.SetResult(new VerifyNumberResult { VerificationId = id });
				}
			});
			return tcs.Task;
		}

		public Task<AuthorizedUser> SignIn(string verificationId, string verificationCode)
		{
			var tcs = new TaskCompletionSource<AuthorizedUser>();
			var credentional = PhoneAuthProvider.DefaultInstance.GetCredential(verificationId, verificationCode);
			Auth.DefaultInstance.SignIn(credentional, (user, error) =>
			{
				if (error != null)
				{
					tcs.SetException(new Exception(error.LocalizedDescription));
				}
				else
				{
					tcs.SetResult(new AuthorizedUser
					{
						PhoneNumber = user.PhoneNumber,
						RefreshToken = user.RefreshToken,
						Uid = user.Uid
					});
				}
			});

			return tcs.Task;
		}
	}
}