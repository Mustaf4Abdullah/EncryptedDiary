from flask import Flask, request, jsonify
import joblib

# Load model and encoder
model = joblib.load("emotion_model_v2.pkl")
label_encoder = joblib.load("label_encoder_v2.pkl")

app = Flask(__name__)

@app.route('/predict', methods=['POST'])
def predict():
    data = request.json
    text = data.get('text', '')

    if not text.strip():
        return jsonify({'emotion': 'neutral'})

    prediction = model.predict([text])
    label = label_encoder.inverse_transform(prediction)[0]

    return jsonify({'emotion': label})


@app.route('/labels', methods=['GET'])
def get_labels():
    """Get all possible emotion labels that the model can predict"""
    return jsonify({'labels': label_encoder.classes_.tolist()})

if __name__ == '__main__':
    app.run(port=5000)
