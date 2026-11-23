// Varjupaiga maskott: takskoer, kes piilub ja vehib käpaga.
// Alumine serv on lõigatud, et koer paistaks kaardi või künka tagant.
function Dachshund(props) {
  return (
    <svg viewBox="0 0 240 190" className="dachshund" role="img" aria-label="Takskoer vehib käpaga" {...props}>
      <path d="M68 190 C70 150 82 138 110 138 C138 138 150 150 152 190 Z" fill="#8b4f2a" />

      <g className="dog-head">
        <ellipse cx="110" cy="112" rx="46" ry="44" fill="#9a5b34" />
        <ellipse cx="110" cy="133" rx="27" ry="20" fill="#d9a673" />
        <path d="M70 92 C46 96 34 130 44 160 C60 164 72 146 74 120 Z" fill="#6b3a1d" />
        <path d="M150 92 C174 96 186 130 176 160 C160 164 148 146 146 120 Z" fill="#6b3a1d" />
        <circle cx="93" cy="95" r="4.5" fill="#e6b886" />
        <circle cx="127" cy="95" r="4.5" fill="#e6b886" />
        <g className="dog-eyes">
          <circle cx="93" cy="108" r="6.5" fill="#241510" />
          <circle cx="127" cy="108" r="6.5" fill="#241510" />
          <circle cx="95" cy="106" r="2" fill="#fff" />
          <circle cx="129" cy="106" r="2" fill="#fff" />
        </g>
        <ellipse cx="110" cy="126" rx="9.5" ry="6.5" fill="#241510" />
        <path
          d="M110 132 v6 M110 138 c-5 6 -13 5 -15 -1 M110 138 c5 6 13 5 15 -1"
          stroke="#241510"
          strokeWidth="2.4"
          fill="none"
          strokeLinecap="round"
        />
        <path d="M103 143 q7 16 14 0 z" fill="#ef7b86" />
      </g>

      <g className="dog-paw">
        <rect x="172" y="102" width="26" height="90" rx="13" fill="#9a5b34" />
        <ellipse cx="185" cy="112" rx="15" ry="13" fill="#b06a40" />
        <circle cx="179" cy="108" r="2.6" fill="#6b3a1d" />
        <circle cx="185" cy="105" r="2.6" fill="#6b3a1d" />
        <circle cx="191" cy="108" r="2.6" fill="#6b3a1d" />
      </g>
    </svg>
  )
}

export default Dachshund
