using API.DTOs;
using API.Entities;
using API.Extentions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class MessagesController : BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly IMapper _mapper;
        public MessagesController(IUserRepository userRepository, IMessageRepository messageRepository, IMapper mapper)
        {
            _mapper = mapper;
            _messageRepository = messageRepository;
            _userRepository = userRepository;
        }

        [HttpPost]
        public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto creatMessageDto)
        {
            var username = User.GetUsername();

            if (username == creatMessageDto.RecipientUsername.ToLower())
                return BadRequest("You cannot send message to yourself");

            var sender = await _userRepository.GetUserByUsernameAsync(username);
            var recipient = await _userRepository.GetUserByUsernameAsync(creatMessageDto.RecipientUsername);

            if (recipient == null) return NotFound();

            var messsage = new Message
            {
                Sender = sender,
                Recipient = recipient,
                SenderUsername = sender.Username,
                RecipientUsername = recipient.Username,
                Content = creatMessageDto.Content
            };

            _messageRepository.AddMessage(messsage);

            if (await _messageRepository.SaveAllAsync()) return Ok(_mapper.Map<MessageDto>(messsage));

            return BadRequest("Failed to send message");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessagesForUser([FromQuery]
            MessageParams messageParams)
        {
            messageParams.Username = User.GetUsername();
            var messages = await _messageRepository.GetMessageForUser(messageParams);

            Response.AddPaginationHeader(messages.CurrentPage,messages.PageSize,
                messages.TotalCount,messages.TotalPage);

            return messages;
        }

        [HttpGet("thread/{username}")]
        public async Task<ActionResult<IEnumerable<MessageDto>>>GetMessageThread(string username)
        {
            var currentUsername=User.GetUsername();

            return Ok(await _messageRepository.GetMessageThread(currentUsername,username));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult>DeleteMessage(int id)
        {
            var username =User.GetUsername();

            var message =await _messageRepository.GetMessage(id);

            if(message.Sender.Username!=username&&message.Recipient.Username!=username)
                return Unauthorized();

            if(message.Sender.Username==username) message.SenderDeleted = true;
            if(message.Recipient.Username==username) message.RecipientDeleted=true;

            if(message.SenderDeleted&&message.RecipientDeleted)
                _messageRepository.DeleteMessage(message);

            if(await _messageRepository.SaveAllAsync()) return Ok();

            return BadRequest("Problem deleting the message");
        }
    }
}