using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Aranzadi.DocumentAnalysis.Models
{
    public class OperationResult
    {
        public enum ResultCode { Success, GenericError, NotFound, InvalidData, InvalidState, Forbidden, Throttled }
        public ResultCode Code { get; set; }

        public string? Detail { get; set; }

        /// <summary>
        /// Success result
        /// </summary>
        /// <param name="detail">additional details, such as error traces or any other info. 
        /// This can be used for better error tracing</param>
        /// <returns></returns>
        public static OperationResult Success(string? detail = null)
        {
            return new OperationResult { Code = ResultCode.Success, Detail = detail };
        }


        /// <summary>
        /// Generic error result. Should be used when no other error type fits.
        /// </summary>
        /// <param name="detail">additional details, such as error traces or any other info. 
        /// This can be used for better error tracing</param>
        /// <returns></returns>
        public static OperationResult GenericError(string? detail = null)
        {
            return new OperationResult { Code = ResultCode.GenericError, Detail = detail };
        }

        /// <summary>
        /// Not found result. Should be used when an element referenced in an operation does not exist.
        /// </summary>
        /// <param name="detail">additional details, such as error traces or any other info. 
        /// This can be used for better error tracing</param>
        /// <returns></returns>
        public static OperationResult NotFound(string? detail = null)
        {
            return new OperationResult { Code = ResultCode.NotFound, Detail = detail };
        }

        /// <summary>
        /// Invalid data result. Should be used when its corresponding operation was badly formulated
        /// by the code requesting it.
        /// (ex: missing mandatory inputs, inconsistent inputs, etc.)
        /// </summary>
        /// <param name="detail">additional details, such as error traces or any other info. 
        /// This can be used for better error tracing</param>
        /// <returns></returns>
        public static OperationResult InvalidData(string? detail = null)
        {
            return new OperationResult { Code = ResultCode.InvalidData, Detail = detail };
        }

        /// <summary>
        /// Invalid state result. Should be used when the current state of things makes it impossible
        /// to perform the requested operation (ex: trying to delete an open file)
        /// </summary>
        /// <param name="detail">additional details, such as error traces or any other info. 
        /// This can be used for better error tracing</param>
        /// <returns></returns>
        public static OperationResult InvalidState(string? detail = null)
        {
            return new OperationResult { Code = ResultCode.InvalidState, Detail = detail };
        }

        /// <summary>
        /// Forbidden result.
        /// </summary>
        /// <param name="detail">additional details, such as error traces or any other info. 
        /// This can be used for better error tracing</param>
        /// <returns></returns>
        public static OperationResult Forbidden(string? detail = null)
        {
            return new OperationResult { Code = ResultCode.Forbidden, Detail = detail };
        }

        /// <summary>
        /// Throttled result.
        /// </summary>
        /// <param name="detail">additional details, such as error traces or any other info. 
        /// This can be used for better error tracing</param>
        /// <returns></returns>
        public static OperationResult Throttled(string? detail = null)
        {
            return new OperationResult { Code = ResultCode.Throttled, Detail = detail };
        }
    }

    public class OperationResult<T> : OperationResult
    {

        public T Result { get; set; }

        /// <summary>
        /// Success result
        /// </summary>
        /// <param name="result">object to return</param>
        /// <param name="detail">additional details, such as error traces or any other info. 
        /// This can be used for better error tracing</param>
        /// <returns></returns>
        public static OperationResult<T> Success(T result = default(T), string? detail = null)
        {
            return CodeResult(ResultCode.Success, result, detail);
        }

        /// <summary>
        /// Not found result. Should be used when an element referenced in an operation does not exist.
        /// </summary>
        /// <param name="result">object to return</param>
        /// <param name="detail">additional details, such as error traces or any other info. 
        /// This can be used for better error tracing</param>
        /// <returns></returns>
        public static OperationResult<T> NotFound(T result = default(T), string? detail = null)
        {
            return CodeResult(ResultCode.NotFound, result, detail);
        }

        /// <summary>
        /// Generic error result. Should be used when no other error type fits.
        /// </summary>
        /// <param name="result">object to return</param>
        /// <param name="detail">additional details, such as error traces or any other info. 
        /// This can be used for better error tracing</param>
        /// <returns></returns>
        public static OperationResult<T> GenericError(T result = default(T), string? detail = null)
        {
            return CodeResult(ResultCode.GenericError, result, detail);
        }

        /// <summary>
        /// Invalid data result. Should be used when its corresponding operation was badly formulated
        /// by the code requesting it.
        /// (ex: missing mandatory inputs, inconsistent inputs, etc.)
        /// </summary>
        /// <param name="result">object to return</param>
        /// <param name="detail">additional details, such as error traces or any other info. 
        /// This can be used for better error tracing</param>
        /// <returns></returns>
        public static OperationResult<T> InvalidData(T result = default(T), string? detail = null)
        {
            return CodeResult(ResultCode.InvalidData, result, detail);
        }

        /// <summary>
        /// Invalid state result. Should be used when the current state of things makes it impossible
        /// to perform the requested operation (ex: trying to delete an open file)
        /// </summary>
        /// <param name="result">object to return</param>
        /// <param name="detail">additional details, such as error traces or any other info. 
        /// This can be used for better error tracing</param>
        /// <returns></returns>
        public static OperationResult<T> InvalidState(T result = default(T), string? detail = null)
        {
            return CodeResult(ResultCode.InvalidState, result, detail);
        }

        /// <summary>
        /// Forbidden result.
        /// </summary>
        /// <param name="result">object to return</param>
        /// <param name="detail">additional details, such as error traces or any other info. 
        /// This can be used for better error tracing</param>
        /// <returns></returns>
        public static OperationResult<T> Forbidden(T result = default(T), string? detail = null)
        {
            return CodeResult(ResultCode.Forbidden, result, detail);
        }

        /// <summary>
        /// Throttled result.
        /// </summary>
        /// <param name="result">object to return</param>
        /// <param name="detail">additional details, such as error traces or any other info. 
        /// This can be used for better error tracing</param>
        /// <returns></returns>
        public static OperationResult<T> Throttled(T result = default(T), string? detail = null)
        {
            return CodeResult(ResultCode.Throttled, result, detail);
        }

        /// <summary>
        /// Returns a result for the specified code
        /// </summary>
        /// <param name="resultCode">code</param>
        /// <param name="result">object to return</param>
        /// <param name="detail">additional details, such as error traces or any other info. 
        /// This can be used for better error tracing</param>
        /// <returns></returns>
        private static OperationResult<T> CodeResult(ResultCode resultCode, T result, string? detail = null)
        {
            var res = new OperationResult<T> { Code = resultCode };
            res.Result = result;
            res.Detail = detail;
            return res;
        }

    }

    public static class OperationResultHttpResponseMessageExtensions
    {

        public static async Task<OperationResult> ToOperationResult(this HttpResponseMessage httpResponse)
        {
            OperationResult op = httpResponse.StatusCode.IsSuccess() ? OperationResult.Success() : OperationResult.GenericError();
            var details = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
            switch (httpResponse.StatusCode)
            {
                case HttpStatusCode.NotFound: op = OperationResult.NotFound(details); break;
                case HttpStatusCode.Conflict: op = OperationResult.InvalidState(details); break;
                case HttpStatusCode.BadRequest: op = OperationResult.InvalidData(details); break;
                case (System.Net.HttpStatusCode)429: op = OperationResult.Throttled(details); break;
            }
            return op;
        }

        public static async Task<OperationResult<T>> ToOperationResult<T>(this HttpResponseMessage httpResponse)
        {
            var content = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
            OperationResult<T> op = httpResponse.StatusCode.IsSuccess() ? OperationResult<T>.Success(JsonConvert.DeserializeObject<T>(content)) : OperationResult<T>.GenericError(detail: content);
            switch (httpResponse.StatusCode)
            {
                case HttpStatusCode.NotFound: op = OperationResult<T>.NotFound(detail: content); break;
                case HttpStatusCode.Conflict: op = OperationResult<T>.InvalidState(detail: content); break;
                case HttpStatusCode.BadRequest: op = OperationResult<T>.InvalidData(detail: content); break;
                case (System.Net.HttpStatusCode)429: op = OperationResult<T>.Throttled(detail: content); break;
            }
            return op;
        }

        public static bool IsSuccess(this HttpStatusCode code)
        {
            return (int)code >= 200 && (int)code <= 299;
        }

        public static string GetFileName(this HttpResponseMessage httpResponse)
        {
            if (httpResponse.Content.Headers.ContentDisposition != null && !string.IsNullOrEmpty(httpResponse.Content.Headers.ContentDisposition.FileName))
                return httpResponse.Content.Headers.ContentDisposition.FileName.Replace("\"", "") ?? "";
            return "";
        }

    }
}
