export class Toast {
	/**
	 *
	 * @param {string} text
	 * @param {"info"|"error"|"success"} type
	 */
	static show = (text, type) => {
		const el = document.createElement("div");
		el.classList.add("toast");
		el.classList.add("toast-" + type);
		const p = document.createElement("p");
		p.innerText = text;
		p.innerHTML = p.innerHTML.replace(/\<br\/?>/g, "<br>");
		el.appendChild(p);
		const btn = document.createElement("button");
		btn.onclick = () => el.remove();
		btn.innerText = "X";
		el.appendChild(btn);
		document.querySelector("#toast-container").appendChild(el);
	};
}
