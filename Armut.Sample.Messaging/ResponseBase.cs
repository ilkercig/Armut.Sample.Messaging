using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Armut.Sample.Messaging
{
    public class ResponseBase                                                                                                                        
    {
        private List<string> m_Errors = new List<string>();


        public List<string> ErrorMessages
        {
            get
            {
                return m_Errors;
            }
            set
            {
                m_Errors = value;
            }
        }

        public void AddErrorMessages(string error)
        {
            m_Errors.Add(error);
        }

        public void AddErrorMessages(ModelStateDictionary modelState)
        {
            foreach (var entry in modelState)
            {
                foreach (var error in entry.Value.Errors)
                {
                    if(string.IsNullOrEmpty(entry.Key))
                        m_Errors.Add(error.ErrorMessage);
                    else
                        m_Errors.Add(string.Format("{0} : {1}",entry.Key, error.ErrorMessage));
                }
            }
        }
    }
}
