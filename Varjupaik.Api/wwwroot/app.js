// Ühised funktsioonid kõikidele lehtedele

function getUser() {
    const data = localStorage.getItem("user");
    return data ? JSON.parse(data) : null;
}

function saveUser(user) {
    localStorage.setItem("user", JSON.stringify(user));
}

function logout() {
    localStorage.removeItem("user");
    window.location.href = "index.html";
}

// fetch, mis lisab JWT tokeni päisesse
async function api(url, options = {}) {
    const user = getUser();
    options.headers = options.headers || {};
    if (user) {
        options.headers["Authorization"] = "Bearer " + user.token;
    }
    return fetch(url, options);
}

async function errorText(response) {
    try {
        const data = await response.json();
        if (data.message) return data.message;
        if (data.errors) return Object.values(data.errors).flat().join(", ");
        return data.title || "Viga";
    } catch {
        return "Viga (" + response.status + ")";
    }
}

const statusNames = {
    Available: "Saadaval",
    Reserved: "Broneeritud",
    Adopted: "Adopteeritud"
};

// Päise menüü vastavalt sellele, kas kasutaja on sisse logitud
function showMenu() {
    const menu = document.getElementById("menu");
    const user = getUser();
    if (!user) {
        menu.innerHTML = '<a href="index.html">Loomad</a><a href="login.html">Logi sisse</a>';
        return;
    }
    let html = '<a href="index.html">Loomad</a>';
    if (user.role === "Admin" || user.role === "Staff") {
        html += '<a href="haldus.html">Haldus</a>';
    }
    html += ' <span style="margin-left:12px">' + user.name + " (" + user.role + ")</span>";
    html += '<a href="#" onclick="logout()">Logi välja</a>';
    menu.innerHTML = html;
}

// Looma pilt; kui pilti pole, näitame halli kasti tekstiga
function animalImage(animal) {
    if (!animal.photoUrl) {
        const box = document.createElement("div");
        box.className = "no-photo";
        box.textContent = "Pilt puudub";
        return box;
    }
    const img = document.createElement("img");
    img.src = animal.photoUrl;
    img.alt = animal.name;
    return img;
}
