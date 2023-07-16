const nameInput = document.querySelector("#name");
const filenameInput = document.querySelector("#filename");
let fileExtension = "";

document.querySelector("#name").addEventListener("input", function () {
	filenameInput.placeholder = this.value + "." + fileExtension;
});

document.querySelector("#file").addEventListener("change", function () {
	if (this.files.length > 0) {
		fileExtension = this.files[0].name.split(".").pop().toLowerCase();
		if (nameInput.value === "") {
			nameInput.value = this.files[0].name.split(".").shift();
			filenameInput.placeholder = this.files[0].name;
		} else {
			filenameInput.placeholder = nameInput.value + "." + fileExtension;
		}
	}
});

document.querySelector("#link").addEventListener("input", function () {
	if (this.value) {
		nameInput.placeholder = "";
		filenameInput.placeholder = "";
		if (!nameInput.value) nameInput.value = "";
		if (!filenameInput.value) filenameInput.value = "";
		fetch(this.value).then(r => {
			if (r.ok) {
				const t = r.headers.get("content-type");
				fileExtension = t.split("/").pop();
				filenameInput.placeholder = nameInput.value + "." + fileExtension;
			}
		});
	}
});

document.querySelector("form").addEventListener("submit", async function (e) {
	e.preventDefault();
	document.querySelector(".spinner").style.display = "";

	if (!this.link.value && !this.file.value) {
		this.file.required = true;
		this.link.required = true;
	}

	if (!this.reportValidity()) return;

	if (this.link.value) {
		// download link from the interwebz and set the value to file input
		const response = await fetch(link.value);
		if (!response.ok) {
			appendOutput("Error downloading linked file", "error");
			return;
		}
		const blob = await response.blob();
		const file = new File([blob], "downloaded." + blob.type.split("/").pop(), { type: blob.type });

		const dt = new DataTransfer();
		dt.items.add(file);
		this.file.files = dt.files;
	}

	const form = new FormData(this);

	let contentType = this.Type.value;
	let types = {
		Unknown: 0,
		Other: 1,
		Image: 2,
		Video: 3,
	};

	if (contentType == "-1") {
		// auto
		let file = form.get("File");
		let ext = file.name.split(".").pop().toLowerCase();
		console.log(file.name, file);
		switch (ext) {
			case "jpg":
			case "jpeg":
			case "png":
			case "gif":
				contentType = types.Image;
				break;
			case "mp4":
			case "webm":
			case "ogg":
				contentType = types.Video;
				break;
			default:
				contentType = types.Unknown;
				break;
		}
	}

	if (contentType == types.Unknown) {
		alert("Unknown file type, please select a type");
		return false;
	}

	if (contentType == types.Image) {
		// get width
		let file = form.get("File");
		let img = new Image();

		img.src = URL.createObjectURL(file);
		await new Promise(r => setTimeout(r, 100)); // :tf:

		form.set("Width", img.width);
		form.set("Height", img.height);
	}

	form.set("Type", parseInt(contentType));
	if (form.get("Filename") == "") form.set("Filename", this.Filename.placeholder);

	console.log(form);

	const response = await fetch("/api/upload", {
		method: "POST",
		headers: {
			Accept: "application/json",
		},
		body: form,
	});

	const json = await response.json();

	if (!response.ok) {
		console.error(response);
		console.error(json);
		if (json.errors) {
			// json.errors is an object
			const errors = Object.entries(json.errors);
			for (const [key, value] of errors) {
				console.log(key, value);
				appendOutput(`${key}: ${value.join("; ")}`, "error");
			}
		} else {
			appendOutput(json, "error");
		}
	} else {
		const url = new URL(window.location);
		url.pathname = "/content/" + json.username + "/" + json.contentName;
		appendOutput(`ok: ${url.toString()}`, "success");
	}

	await new Promise(r => setTimeout(r, 10000));
	document.querySelector(".spinner").style.display = "none";
	return false;
});

/**
 * @param {"info"|"error"|"success"} type
 *
 */
function appendOutput(text, type) {
	const span = document.createElement("span");
	span.innerText = `${[new Date().toLocaleString()]}: ${text}`;
	switch (type) {
		case "info":
			span.style.color = "darkblue";
			break;
		case "error":
			span.style.color = "darkred";
			break;
		case "success":
			span.style.color = "darkgreen";
			break;
		default:
			span.style.color = "black";
			break;
	}
	document.querySelector("#output").appendChild(span);
}
