import React, { useEffect, useRef, useCallback, useState } from "react";

import "./App.css";

function App() {
  const messageArray = [
    {
      message: "Some sample message 1",
      from: { username: "John Dou" },
      to: { username: "Some User" },
      type: "incoming",
    },
    {
      message: "Some sample message 2",
      from: { username: "John Dou" },
      to: { username: "Some User" },
      type: "outgoing",
    },
    {
      message: "Some sample message 3",
      from: { username: "John Dou" },
      to: { username: "Some User" },
      type: "incoming",
    },
    {
      message: "Some sample message 4",
      from: { username: "John Dou" },
      to: { username: "Some User" },
      type: "incoming",
    },
    {
      message: "Some sample message 5",
      from: { username: "John Dou" },
      to: { username: "Some User" },
      type: "outgoing",
    },
  ];

  const [messages, setMessages] = useState(messageArray);
  const [message, setMessage] = useState("");

  const divRef = useRef(null);

  const onChangeMessage = useCallback((e) => {
    setMessage(e.target.value);
  }, []);

  const onClickSend = useCallback(() => {
    if (message) {
      setMessages([
        ...messages,
        {
          message,
          from: { username: "John Dou" },
          to: { username: "Some User" },
          type: "outgoing",
        },
      ]);
      setMessage("");
    }
  }, [message, messages]);

  useEffect(() => {
    divRef.current.scrollIntoView();
  });

  return (
    <div className="App">
      <div className="Title">Sample Teams chat</div>
      <div className="ChatWrap">
        <div className="MessagesBox">
          {messages.map((e, i) =>
            e.type === "incoming" ? (
              <div key={i} className="Message">
                <div>
                  <div className="Username">{e.from.username}</div>
                  <div>{e.message}</div>
                </div>
              </div>
            ) : (
              <div key={i} className="Message Message_Outgoing">
                <div className="MessageWrap">
                  <div className="Username">{e.to.username}(you)</div>
                  <div>{e.message}</div>
                </div>
              </div>
            )
          )}
          <div ref={divRef} />
        </div>
        <div className="MessagesSend">
          <textarea value={message} onChange={onChangeMessage} className="Textarea" />
          <button onClick={onClickSend} className="SendButton">
            Send
          </button>
        </div>
      </div>
    </div>
  );
}

export default App;
