using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Dto
{
    public class ErrorDto
    {
        public List<String> Errors { get; private set; }
        public bool IsShow { get; private set; }

        public ErrorDto()
        {
            Errors = new List<string>();
        }
        public ErrorDto(string error,bool isShow)
        {
            Errors.Add(error);
            IsShow = true;
        }
        public ErrorDto(List<string> errors, bool isShow)
        {
            Errors= errors;
            IsShow = true;
        }

    }
}
