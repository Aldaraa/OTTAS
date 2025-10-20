import { useEffect, useRef } from "react";

export default function useShortcut(key, cb) {
  const callback = useRef(cb);

  useEffect(() => {
    callback.current = cb;
  },[cb])

  useEffect(() => {
    function handle(event){
      if (key === 'safeMode' && event.keyCode === 192 && event.altKey && event.shiftKey) {
        callback.current(event);
      }
    }
    document.addEventListener('keydown',handle);
    return () => document.removeEventListener("keydown",handle)
  },[key])
}
