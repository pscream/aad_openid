import axios from "axios";

const instance = axios.create({
  baseURL: process.env.REACT_APP_API_URI,
  headers: {
    Accept: "application/json",
    "Content-Type": "application/json",
  },
});

const api = {
  getChats: (memberId) => instance.get(`api/chat?memberId=${memberId}`),
  getMessages: (chatId, data) =>
    instance.post(`api/chat/${chatId}/getMessagesOnBehalf`, data),
  createChat: (ownerId, memberId) =>
    instance.post(`api/chat?ownerId=${ownerId}&memberId=${memberId}`),
  sendToChat: (chatId, data) =>
    instance.post(`api/chat/${chatId}/messageOnBehalf`, data), // data: { message, token }
  deleteChat: (chatId) => instance.delete(`api/chat/${chatId}`),
};

export default api;
