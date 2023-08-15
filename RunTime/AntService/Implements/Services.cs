using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ant.MetaVerse
{
    public class PaymentService : IPaymentService
    {
        public void Pay(string transactionId, Action<Exception> callback)
        {
            try{
                Debug.Log("Pay");
            }
            catch(Exception e){
                callback(e);
            }
        }

        ~PaymentService(){
            Debug.Log("PaymentService's destructor");
        }
    }

    public class UserService : IUserService
    {
        public void GetAuthCode(Action<Exception, string> callback)
        {
            try{
                Debug.Log("GetAuthCode");
            }
            catch(Exception e){
                callback(e, null);
            }
        }

        public void GetHealthData(DateTime date, Action<Exception, string> callback)
        {
            try{
                Debug.Log("GetHealthData");
            }
            catch(Exception e){
                callback(e, null);
            }
        }

        public void GetFriends(Action<Exception, List<Friend>> callback)
        {
            try{
                Debug.Log("GetFriends");
                callback(null, new List<Friend>(){new Friend()});
            }
            catch(Exception e){
                callback(e, null);
            }
        }

        ~UserService(){
            Debug.Log("UserService's destructor");
        }
    }

}