using Seiun.Resources;

namespace Seiun.Models.Responses;

public struct ParamValidationError
{
	public required string Field { get; set; }
	public required IEnumerable<string>? Errors { get; set; }
}

public sealed class ParamValidationResp(IEnumerable<ParamValidationError> errors)
	: BaseRespWithData<IEnumerable<ParamValidationError>>(400, ErrorMessages.Controller.Any.ParamValidFailed, errors)
{
}
