import React, { useEffect, useRef, useCallback, useState } from "react";
import { useQuery } from "react-query";
import { useQueryClient } from "react-query";

import "./App.css";
import api from "./api";

function Chat() {
  const [message, setMessage] = useState("");
  const [selectedChat, setSelectedChat] = useState("");
  const queryClient = useQueryClient()

  const accessToken = localStorage.getItem("accessToken");
  const preferredName = localStorage.getItem("preferredName");

  const chatsResponse = useQuery(
    "chats",
    async () => await api.getChats(preferredName),
    { staleTime: Infinity }
  );

  const messagesResponse = useQuery(
    `messages ${selectedChat}`,
    async () => await api.getMessages(selectedChat, { token: accessToken }),
    { staleTime: Infinity }
  );

  const chats = chatsResponse?.data?.data?.split("\n");
  const messages = messagesResponse?.data?.data?.split("\n");

  if (chats?.length) {
    chats.shift();
  }

  if (messages?.length) {
    messages.shift();
    messages.reverse();
  }

  const divRef = useRef(null);

  const onChangeMessage = useCallback((e) => {
    setMessage(e.target.value);
  }, []);

  const refreshChats = useCallback(() => {
    queryClient.refetchQueries("chats");
    queryClient.refetchQueries(`messages ${selectedChat}`);
  }, [queryClient, selectedChat]);

  const onClickSend = useCallback(async () => {
    if (message) {
      await api.sendToChat(selectedChat, {
        token: accessToken,
        message,
      });
      setMessage("");
      refreshChats();
    }
  }, [message, selectedChat, accessToken, refreshChats]);

  useEffect(() => {
    divRef.current.scrollIntoView();
  });

  return (
    <div className="App">
      <div className="Title">Sample Teams chat</div>
      <div className="ChatWrap">
        <div className="ChatsBox">
          <div className="Chats">
            {chats?.map((e, i) => (
              <div
                key={e}
                onClick={() => setSelectedChat(e)}
                className={`Chat ${e === selectedChat ? "Chat_selected" : ""}`}
                title={e}
              >
                Chat {i + 1}
              </div>
            ))}
          </div>
          <div className="MessagesBox">
            <div onClick={refreshChats} className="RefreshChat">Refresh chat</div>
            <div className="Gap"></div>
            {selectedChat
              ? messages?.map((e) => (
                  <div key={e} dangerouslySetInnerHTML={{ __html: e }}></div>
                ))
              : "Select a chat"}
            <div ref={divRef} />
          </div>
        </div>
        <div className="MessagesSend">
          <textarea
            value={message}
            onChange={onChangeMessage}
            className="Textarea"
          />
          <button onClick={onClickSend} className="SendButton">
            Send
          </button>
        </div>
      </div>
    </div>
  );
}

export default Chat;
