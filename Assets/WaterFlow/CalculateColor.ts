let v = 0;
let a = 0;

let v_ms = v * 0.514444;
let sin = Math.sin(0.017453293 * a);
let cos = Math.cos(0.017453293 * a);

const max_v = 1;
let v_n = v_ms / max_v;
let y = sin * v_n;
let x = cos * v_n;

let r = (x + 1) * 128;
let g = (y + 1) * 128;
x;
y;
console.log(Math.floor(r).toString(16)+Math.floor(g).toString(16)+"00")

let rr = 0.655*2-1;
let gg = 0.592*2-1;

console.log(Math.sqrt(rr*rr+gg*gg)/0.514444)
