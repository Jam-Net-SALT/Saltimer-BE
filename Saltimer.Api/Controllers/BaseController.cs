
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Saltimer.Api.Controllers
{
    [Route("api/[controller]"), Authorize]
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
        protected readonly SaltimerDBContext _context;
        protected readonly IAuthService _authService;
        protected readonly IMapper _mapper;

        public BaseController(IMapper mapper, IAuthService authService, SaltimerDBContext context)
        {
            _mapper = mapper;
            _authService = authService;
            _context = context;
        }
    }
}