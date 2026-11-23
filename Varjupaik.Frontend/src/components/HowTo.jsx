import { DocIcon, HomeIcon, SearchIcon } from './Icons'

const steps = [
  { Icon: SearchIcon, title: 'Leia oma lemmik', text: 'Sirvi loomi ja kasuta filtreid liigi, oleku ja nime järgi.' },
  { Icon: DocIcon, title: 'Esita taotlus', text: 'Loo konto ja kirjuta paar rida endast ning oma kodust.' },
  { Icon: HomeIcon, title: 'Vii sõber koju', text: 'Meie töötajad vaatavad taotluse üle ja võtavad sinuga ühendust.' },
]

function HowTo() {
  return (
    <section id="kuidas" className="band band-white">
      <div className="container">
        <div className="section-head">
          <div>
            <span className="eyebrow">Kolm lihtsat sammu</span>
            <h2>Kuidas adopteerida</h2>
          </div>
        </div>

        <div className="steps">
          {steps.map(({ Icon, title, text }, i) => (
            <div className="step" key={title}>
              <span className="step-number">{i + 1}</span>
              <span className="step-icon"><Icon /></span>
              <h3>{title}</h3>
              <p>{text}</p>
            </div>
          ))}
        </div>
      </div>
    </section>
  )
}

export default HowTo
